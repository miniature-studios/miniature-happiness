#if JSONNET_EXISTS
using Newtonsoft.Json;
#endif

using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class ProjectCacher : MonoBehaviourBinder<FigmaConverterUnity>
    {
        private IEnumerator restoreCacheRoutine = null;
        public void TryRestoreProjectFromCache(string projectId = null)
        {
            restoreCacheRoutine?.StopDARoutine();
            restoreCacheRoutine = _TryRestoreProjectFromCache(projectId);
            restoreCacheRoutine.StartDARoutine(monoBeh);
        }

        private IEnumerator _TryRestoreProjectFromCache(string projectId)
        {
            if (projectId.IsEmpty())
            {
                monoBeh.Log($"AssetCache | projectId.IsEmpty");
                yield break;
            }

            if (monoBeh.CurrentProject.FigmaProject.IsProjectEmpty() == false)
            {
                monoBeh.Log($"AssetCache | IsProjectEmpty() == false");
                yield break;
            }

            List<CacheMeta> cache = GetAll();

            if (cache.IsEmpty())
            {
                monoBeh.Log($"AssetCache | cache.IsEmpty");
                yield break;
            }

            CacheMeta cachedFileMeta = cache.FirstOrDefault(x => x.Url == projectId);

            if (cachedFileMeta.IsDefault())
            {
                monoBeh.Log($"AssetCache | no cache for {projectId}");
                yield break;
            }

            bool get = GetCachePath(cachedFileMeta.FileName).TryReadAllText(out string json);

            if (get == false)
            {
                monoBeh.Log($"AssetCache | can't read cache json | {GetCachePath(cachedFileMeta.FileName)}");
                yield break;
            }

            DALogger.Log(FcuLocKey.log_loading_from_cache.Localize(cachedFileMeta.Name));

            FigmaProject jsonParseResult = DAJson.FromJson<FigmaProject>(json);

            if (jsonParseResult.IsProjectEmpty())
            {
                monoBeh.Log($"AssetCache | if (figmaProject.IsEmpty())\n{json}");
                yield break;
            }

            monoBeh.CurrentProject.FigmaProject = jsonParseResult;
            monoBeh.InspectorDrawer.FillSelectableFramesArray();

            DALogger.Log(FcuLocKey.log_cache_restored.Localize(monoBeh.CurrentProject.FigmaProject.Document.Children.Count()));
        }


        public IEnumerator Cache<T>(string json, T @object, string projectUrl)
        {
            FigmaProject figmaProject = (FigmaProject)Convert.ChangeType(@object, typeof(FigmaProject));

            CacheMeta projectMeta = new CacheMeta
            {
                Url = projectUrl,
                Name = figmaProject.Name,
                DateTime = DateTime.Now
            };

            List<CacheMeta> cache = GetAll();

            ClearCacheIfLarge(cache);
            RemoveOldCacheItem(cache, projectMeta);

            string safeFileName = projectMeta.Name.ReplaceInvalidFileNameChars();
            string universalFileName = $"{DateTime.Now.ToString(FcuConfig.Instance.DateTimeFormat1)}_{safeFileName}";

            string projectFileName = universalFileName + $".{CacheType.Json.ToLower()}";
            string metaFileName = universalFileName + $".{CacheType.FCache.ToLower()}";

            string projectFilePath = GetCachePath(projectFileName);
            string metaFilePath = GetCachePath(metaFileName);

            projectMeta.FileName = projectFileName;

            string metaJson = DAJson.ToJson(projectMeta);
            File.WriteAllText(metaFilePath, metaJson);

            JFResult jfr = DAFormatter.Format<FigmaProject>(json);
            File.WriteAllText(projectFilePath, jfr.Json);

            yield return null;
        }


        public List<CacheMeta> GetAll()
        {
            FileInfo[] fileInfos = new DirectoryInfo(FcuConfig.CachePath)
                .GetFiles($"*.{CacheType.FCache.ToLower()}");

            List<CacheMeta> metas = new List<CacheMeta>();
#if JSONNET_EXISTS
            foreach (FileInfo fileInfo in fileInfos)
            {
                string json = File.ReadAllText(fileInfo.FullName);

                CacheMeta meta = JsonConvert.DeserializeObject<CacheMeta>(json);
                metas.Add(meta);

            }

            metas = metas.OrderByDescending(x => x.DateTime).ToList();
#endif
            return metas;
        }

        private void RemoveOldCacheItem(List<CacheMeta> cache, CacheMeta newCache)
        {
            foreach (var item in cache)
            {
                if (item.Url == newCache.Url && item.DateTime.Equals(newCache.DateTime) == false)
                {
                    RemoveCacheItem(item);
                    break;
                }
            }
        }

        private void RemoveCacheItem(CacheMeta cacheMeta)
        {
            string filenameWithoutEx = Path.GetFileNameWithoutExtension(cacheMeta.FileName);

            string fullPathJson = GetCachePath($"{filenameWithoutEx}.{CacheType.Json.ToLower()}");
            string fullPathCache = GetCachePath($"{filenameWithoutEx}.{CacheType.FCache.ToLower()}");

            if (File.Exists(fullPathJson))
                File.Delete(fullPathJson);

            if (File.Exists(fullPathCache))
                File.Delete(fullPathCache);
        }

        private void ClearCacheIfLarge(List<CacheMeta> metas)
        {
            if (metas.Count() > FcuConfig.Instance.MaxCachedFilesCount)
            {
                metas = metas.OrderByDescending(x => x.DateTime).ToList();

                List<CacheMeta> notremove = metas.GetRange(0, FcuConfig.Instance.MaxCachedFilesCount);
                List<CacheMeta> toremove = metas.Exclude(notremove, i => i.Name).ToList();

                foreach (CacheMeta cacheMeta in toremove)
                {
                    RemoveCacheItem(cacheMeta);
                }
            }
        }

        public static string GetCachePath(string fileName) => Path.Combine(FcuConfig.CachePath, fileName);
    }

    internal enum CacheType
    {
        Json,
        FCache,
        Log
    }

    public struct CacheMeta
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
        public string FileName { get; set; }
    }
}