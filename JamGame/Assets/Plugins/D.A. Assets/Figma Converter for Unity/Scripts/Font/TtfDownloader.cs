using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace DA_Assets.FCU
{
    [Serializable]
    public class TtfDownloader : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public IEnumerator Download(List<FontMetadata> figmaFonts)
        {
            UnityFonts unityTtfFonts = monoBeh.FontDownloader.FindUnityFonts(figmaFonts, monoBeh.FontLoader.TtfFonts);

            if (unityTtfFonts.Missing.Count == 0)
                yield break;

            yield return monoBeh.FontDownloader.GFontsApi.GetGoogleFontsBySubset(monoBeh.FontDownloader.GFontsApi.FontSubsets);

            List<FontStruct> downloaded = new List<FontStruct>();
            List<FontError> notDownloaded = new List<FontError>();

            foreach (FontStruct missingFont in unityTtfFonts.Missing)
            {
                DownloadFont(missingFont, @return =>
                {
                    downloaded.Add(@return.Object);

                    if (@return.Success == false)
                    {
                        notDownloaded.Add(new FontError
                        {
                            FontStruct = missingFont,
                            Error = @return.Error
                        });
                    }

                }).StartDARoutine(monoBeh);
            }

            int tempCount = -1;
            while (FcuLogger.WriteLogBeforeEqual(downloaded, unityTtfFonts.Missing, FcuLocKey.log_downloading_fonts, ref tempCount))
            {
                yield return WaitFor.Delay1();
            }

            if (notDownloaded.Count > 0)
            {
                monoBeh.FontDownloader.PrintFontNames(FcuLocKey.cant_download_fonts, notDownloaded);
            }

            yield return SaveTtfFonts(downloaded);

            yield return monoBeh.FontLoader.AddToTtfFontsList();
        }


        public IEnumerator DownloadFont(FontStruct missingFont, Return<FontStruct> @return)
        {
            FontItem fontItem = monoBeh.FontDownloader.GFontsApi.GetFontItem(missingFont.FontMetadata, missingFont.FontSubset);

            if (fontItem.IsDefault())
            {
                @return.Invoke(new DAResult<FontStruct>
                {
                    Error = new IDAError(0, "Font not found in Google Fonts"),
                    Success = false
                });

                yield break;
            }

            string fontUrl = monoBeh.FontDownloader.GFontsApi.GetUrlByWeight(
                fontItem,
                missingFont.FontMetadata.Weight,
                missingFont.FontMetadata.FontStyle);

            if (fontUrl.IsEmpty())
            {
                @return.Invoke(new DAResult<FontStruct>
                {
                    Error = new IDAError(0, $"'{missingFont.FontMetadata.Weight.FontWeightToString()}' weight not found in Google Fonts"),
                    Success = false
                });

                yield break;
            }

            DARequest request = new DARequest
            {
                RequestType = RequestType.GetFile,
                Query = fontUrl
            };

            yield return monoBeh.RequestSender.SendRequest<byte[]>(request, (Return<byte[]>)(@return2 =>
            {
                if (return2.Success)
                {
                    FontStruct res = missingFont;
                    res.Bytes = return2.Object;

                    @return.Invoke(new DAResult<FontStruct>
                    {
                        Object = res,
                        Success = true
                    });
                }
                else
                {
                    @return.Invoke(new DAResult<FontStruct>
                    {
                        Error = return2.Error,
                        Success = false
                    });
                }
            }));
        }

        public IEnumerator SaveTtfFonts(List<FontStruct> downloadedFonts)
        {
#if UNITY_EDITOR
            foreach (FontStruct fs in downloadedFonts)
            {
                if (fs.Bytes == null || fs.Bytes.Length < 1)
                {
                    continue;
                }

                try
                {
                    string baseFontName = monoBeh.FontDownloader.GetBaseFileName(fs);

                    string ttfPath = Path.Combine(monoBeh.FontLoader.TtfFontsPath, $"{baseFontName}.ttf");

                    Directory.CreateDirectory(monoBeh.FontLoader.TtfFontsPath);
                    File.WriteAllBytes(ttfPath, fs.Bytes);
                }
                catch (Exception ex)
                {
                    DALogger.LogException(ex);
                }

                yield return null;
            }

            UnityEditor.AssetDatabase.Refresh();
#endif
            yield return null;
        }
    }
}
