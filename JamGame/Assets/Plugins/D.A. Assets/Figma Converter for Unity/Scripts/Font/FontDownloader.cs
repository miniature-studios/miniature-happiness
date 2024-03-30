using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.FCU
{
    [Serializable]
    public class FontDownloader : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public IEnumerator DownloadFonts(List<FObject> fobjects)
        {
            if (FcuConfig.Instance.GoogleFontsApiKey.IsEmpty())
            {
                DALogger.Log(FcuLocKey.log_no_google_fonts_api_key.Localize());
                yield break;
            }

            List<FontMetadata> figmaFonts = GetFigmaProjectFonts(fobjects);

            monoBeh.FontLoader.RemoveDubAndMissingFonts();

            yield return monoBeh.FontLoader.AddToTmpMeshFontsList();
            yield return monoBeh.FontLoader.AddToTtfFontsList();

            yield return this.TtfDownloader.Download(figmaFonts);

            if (monoBeh.UsingTextMeshPro())
            {
                yield return this.TmpDownloader.CreateFonts(figmaFonts);
            }

            monoBeh.FontLoader.RemoveDubAndMissingFonts();
        }

        public UnityFonts FindUnityFonts<T>(List<FontMetadata> figmaFonts, List<T> fontArray) where T : UnityEngine.Object
        {
            UnityFonts uf = new UnityFonts
            {
                Existing = new List<FontStruct>(),
                Missing = new List<FontStruct>(),
            };

            foreach (FontMetadata figmaFont in figmaFonts)
            {
                foreach (FontSubset fontSubset in monoBeh.FontDownloader.GFontsApi.SelectedFontAssets)
                {
                    T _fontItem = null;

                    foreach (T fontItem in fontArray)
                    {
                        if (fontSubset == FontSubset.Latin)
                        {
                            if (figmaFont.FontNameToString(true, true, null, true) == fontItem.name.FormatFontName())
                            {
                                _fontItem = fontItem;
                                break;
                            }
                        }
                        else
                        {
                            if (figmaFont.FontNameToString(true, true, fontSubset, true) == fontItem.name.FormatFontName())
                            {
                                _fontItem = fontItem;
                                break;
                            }
                        }
                    }

                    if (_fontItem != null)
                    {
                        uf.Existing.Add(new FontStruct
                        {
                            FontMetadata = figmaFont,
                            FontSubset = fontSubset,
                            Font = _fontItem
                        });
                    }
                    else
                    {
                        uf.Missing.Add(new FontStruct
                        {
                            FontMetadata = figmaFont,
                            FontSubset = fontSubset,
                            Font = _fontItem
                        });
                    }
                }
            }

            return uf;
        }

        private List<FontMetadata> GetFigmaProjectFonts(List<FObject> fobjects)
        {
            HashSet<FontMetadata> fonts = new HashSet<FontMetadata>();

            foreach (FObject fobject in fobjects)
            {
                if (fobject.ContainsTag(FcuTag.Text) == false)
                    continue;

                FontMetadata fm = fobject.GetFontMetadata();
                fonts.Add(fm);
            }

            return fonts.ToList();
        }

        public IEnumerator DownloadAllProjectFonts()
        {
            FObject virtualPage = new FObject
            {
                Children = monoBeh.CurrentProject.FigmaProject.Document.Children,
                Data = new SyncData
                {
                    NewName = FcuTag.Page.ToString(),
                    Tags = new List<FcuTag>
                    {
                        FcuTag.Page
                    }
                }
            };

            monoBeh.TagSetter.SetTags(virtualPage);

            monoBeh.ProjectImporter.ConvertTreeToList(virtualPage, monoBeh.CurrentProject.CurrentPage);

            yield return monoBeh.FontDownloader.DownloadFonts(monoBeh.CurrentProject.CurrentPage);
        }

        public string GetBaseFileName(FontStruct fs)
        {
            string baseFontName;

            if (fs.FontSubset == FontSubset.Latin)
            {
                baseFontName = fs.FontMetadata.FontNameToString(true, true, null, false);
            }
            else
            {
                baseFontName = fs.FontMetadata.FontNameToString(true, true, fs.FontSubset, false);
            }

            return baseFontName;
        }

        public void PrintFontNames(FcuLocKey locKey, List<FontError> fonts)
        {
            List<string> fontNames = new List<string>();

            foreach (var item in fonts)
            {
                string fontName;

                if (item.FontStruct.FontSubset == FontSubset.Latin)
                {
                    fontName = item.FontStruct.FontMetadata.FontNameToString(true, true, null, false);
                }
                else
                {
                    fontName = item.FontStruct.FontMetadata.FontNameToString(true, true, item.FontStruct.FontSubset, false);
                }

                string nameWithReason;

                if (item.Error.Message.IsEmpty() == false)
                {
                    nameWithReason = $"{fontName} - {item.Error.Message}";
                }
                else if (item.Error.Exception.IsDefault() == false)
                {
                    nameWithReason = $"{fontName} - {item.Error.Exception}";
                }
                else
                {
                    nameWithReason = fontName;
                }

                fontNames.Add(nameWithReason);
            }

            string joined = $"\n{string.Join("\n", fontNames)}";
            DALogger.Log(locKey.Localize(fontNames.Count, joined));
        }


        [SerializeField] TmpDownloader tmpDownloader;
        [SerializeProperty(nameof(tmpDownloader))]
        public TmpDownloader TmpDownloader => tmpDownloader.SetMonoBehaviour(monoBeh);

        [SerializeField] TtfDownloader ttfDownloader;
        [SerializeProperty(nameof(ttfDownloader))]
        public TtfDownloader TtfDownloader => ttfDownloader.SetMonoBehaviour(monoBeh);

        [SerializeField] DaGoogleFontsApi gFontsApi;
        [SerializeProperty(nameof(gFontsApi))]
        public DaGoogleFontsApi GFontsApi => gFontsApi.SetMonoBehaviour(monoBeh);
    }

    [Serializable]
    public struct FontMetadata
    {
        [SerializeField] string family;
        [SerializeField] int weight;
        [SerializeField] FontStyle fontStyle;

        public string Family { get => family; set => family = value; }
        public int Weight { get => weight; set => weight = value; }
        public FontStyle FontStyle { get => fontStyle; set => fontStyle = value; }
    }

    [Serializable]
    public struct FontStruct
    {
        [SerializeField] FontSubset fontSubset;
        [SerializeField] FontItem fontItem;
        [SerializeField] FontMetadata fontMetadata;
        [SerializeField] byte[] bytes;
        [SerializeField] UnityEngine.Object font;

        public FontSubset FontSubset { get => fontSubset; set => fontSubset = value; }
        public FontMetadata FontMetadata { get => fontMetadata; set => fontMetadata = value; }
        public byte[] Bytes { get => bytes; set => bytes = value; }
        public UnityEngine.Object Font { get => font; set => font = value; }
    }

    public struct FontError
    {
        public FontStruct FontStruct { get; set; }
        public IDAError Error { get; set; }
    }
}