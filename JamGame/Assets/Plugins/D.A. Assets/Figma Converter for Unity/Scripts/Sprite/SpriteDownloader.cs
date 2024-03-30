#if JSONNET_EXISTS
using Newtonsoft.Json;
#endif


using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DA_Assets.FCU
{
    [Serializable]
    public class SpriteDownloader : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public IEnumerator SetMissingImageLinks(List<FObject> fobjects, List<IdLink> idLinks)
        {
            int settedLinksCount = 0;

            for (int i = 0; i < fobjects.Count(); i++)
            {
                for (int j = 0; j < idLinks.Count(); j++)
                {
                    if (fobjects[i].Id == idLinks[j].Id)
                    {
                        if (idLinks[j].Link.IsEmpty())
                        {
                            DALogger.LogError($"Can't get link, please check the following component: {fobjects[i].Data.Hierarchy}");
                            fobjects[i].Data.NeedDownload = false;
                        }
                        else
                        {
                            fobjects[i].Data.Link = idLinks[j].Link;
                        }

                        settedLinksCount++;
                    }
                }
            }

            DALogger.Log(FcuLocKey.log_links_added.Localize(settedLinksCount, idLinks.Count()));
            yield return null;
        }

        public IEnumerator GetMissingSpriteLinks(List<FObject> fobjects, Action<List<IdLink>> callback)
        {
            List<string> fobjectIds = fobjects.Select(x => x.Id).ToList();

            List<List<string>> idChunks = fobjectIds.Split(FcuConfig.Instance.ChunkSizeGetSpriteLinks);
            List<List<IdLink>> linkChunks = new List<List<IdLink>>();

            foreach (List<string> chunk in idChunks)
            {
                DARequest request = RequestCreator.CreateImageLinksRequest(monoBeh.Settings.MainSettings.ProjectUrl, chunk, monoBeh);

                GetChunkImageLinks(request, result =>
                {
                    if (result.Success)
                    {
                        linkChunks.Add(result.Object);
                    }
                    else
                    {
                        linkChunks.Add(default);

                        switch (result.Error.Status)
                        {
                            case 404:
                                DALogger.LogError(FcuLocKey.log_cant_get_images.Localize(FcuLocKey.label_figma_comp.Localize()));
                                break;
                            default:
                                DALogger.LogError(FcuLocKey.log_cant_get_image_links.Localize(result.Error.Message, result.Error.Status));
                                break;
                        }
                    }
                }).StartDARoutine(monoBeh);
            }

            int tempCount = -1;
            while (FcuLogger.WriteLogBeforeEqual(idChunks, linkChunks, FcuLocKey.log_getting_links, linkChunks.CountAll(), fobjectIds.Count(), ref tempCount))
            {
                yield return WaitFor.Delay1();
            }

            List<IdLink> missingImageLinks = linkChunks.FromChunks();

            if (FcuConfig.Instance.Https == false)
            {
                missingImageLinks.ForEach(x => x.Link = x.Link?.Replace("https://", "http://"));
            }

            callback.Invoke(missingImageLinks);
        }

        public IEnumerator GetChunkImageLinks(DARequest request, Return<List<IdLink>> @return)
        {
            DAResult<List<IdLink>> routineResult = new DAResult<List<IdLink>>();

            yield return monoBeh.RequestSender.SendRequest<FigmaImageRequest>(request, result =>
            {
                if (result.Success)
                {
                    List<IdLink> chunkImageLinks = new List<IdLink>();

                    if (result.Object.images.IsEmpty())
                        return;

                    foreach (KeyValuePair<string, string> image in result.Object.images)
                    {
                        chunkImageLinks.Add(new IdLink
                        {
                            Id = image.Key,
                            Link = image.Value
                        });
                    }

                    routineResult.Success = true;
                    routineResult.Object = chunkImageLinks;
                }
                else
                {
                    routineResult.Success = false;
                    routineResult.Error = result.Error;
                }
            });

            @return.Invoke(routineResult);
        }

        public IEnumerator DownloadSprites(List<FObject> fobjects)
        {
            List<FObject> needDownload = fobjects.Where(x => x.Data.NeedDownload).ToList();

            if (needDownload.IsEmpty())
                yield break;

            List<IdLink> missingImageLinks = new List<IdLink>();

            yield return GetMissingSpriteLinks(needDownload, @return => missingImageLinks = @return);
            yield return SetMissingImageLinks(needDownload, missingImageLinks);

            DALogger.Log(FcuLocKey.log_start_download_images.Localize());

            int requestCount = 0;

            foreach (List<FObject> chunk in needDownload.Split(FcuConfig.Instance.ChunkSizeDownloadSprites))
            {
                ConcurrentBag<DownloadedSprite> images = new ConcurrentBag<DownloadedSprite>();

                foreach (FObject item in chunk)
                {
                    DownloadSprite(item, images, () => requestCount++);
                }

                int tempCount = -1;
                while (FcuLogger.WriteLogBeforeEqual(images, chunk, FcuLocKey.log_downloading_images, ref tempCount))
                {
                    yield return WaitFor.Delay1();
                }

                foreach (DownloadedSprite image in images)
                {
                    if (image.SpriteBytes == null)
                    {
                        continue;
                    }

                    DALogger.Log($"DownloadSprite | {image.FObject.Data.SpritePath}");
                    File.WriteAllBytes(image.FObject.Data.SpritePath, image.SpriteBytes);
                }
            }
        }

        private void DownloadSprite(FObject item, ConcurrentBag<DownloadedSprite> images, Action callback)
        {
            DARequest request = new DARequest
            {
                RequestType = RequestType.GetFile,
                Query = item.Data.Link
            };

            if (request.Query.IsEmpty())
            {
                DALogger.LogError(
                    FcuLocKey.log_malformed_url.Localize(item.Data.Hierarchy,
                    FcuLocKey.label_components_settings.Localize()));

                images.Add(default);
                return;
            }

            item.Data.DownloadAttempsCount++;

            monoBeh.RequestSender.SendRequest<byte[]>(request, (result) =>
            {
                if (result.Success && result.Object != null && result.Object.Length > 1)
                {
                    images.Add(new DownloadedSprite
                    {
                        FObject = item,
                        SpriteBytes = result.Object
                    });
                }
                else if (item.Data.DownloadAttempsCount < 3)
                {
                    DownloadSprite(item, images, callback);
                    return;
                }
                else
                {
                    images.Add(new DownloadedSprite
                    {
                        SpriteBytes = null
                    });

                    switch (result.Error.Status)
                    {
                        case 909:
                            DALogger.LogError(FcuLocKey.log_ssl_error.Localize(result.Error.Message, result.Error.Status));
                            monoBeh.Events.OnImportFail?.Invoke(monoBeh);
                            monoBeh.AssetTools.StopImport();
                            break;
                        default:
                            DALogger.LogError(FcuLocKey.cant_download_sprite.Localize(item.Data.Hierarchy, result.Error.Message, result.Error.Status));
                            break;
                    }
                }

                callback.Invoke();
            }).StartDARoutine(monoBeh);
        }
    }
    public struct FigmaImageRequest
    {
#if JSONNET_EXISTS
        [JsonProperty("err")]
#endif
        public string error;
#if JSONNET_EXISTS
        [JsonProperty("images")]
#endif
        public Dictionary<string, string> images;
    }

    public struct DownloadedSprite
    {
        public FObject FObject;
        public byte[] SpriteBytes;
    }

    public struct IdLink
    {
        public string Id;
        public string Link;
    }
}
