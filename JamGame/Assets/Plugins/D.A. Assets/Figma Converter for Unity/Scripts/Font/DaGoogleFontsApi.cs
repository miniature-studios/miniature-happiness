using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class DaGoogleFontsApi : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] FontSubset fontSubsets = FontSubset.Latin;
        [SerializeProperty(nameof(fontSubsets))]
        public FontSubset FontSubsets
        {
            get
            {
                return fontSubsets;
            }
            set
            {
                value |= FontSubset.Latin;
                SetValue(ref fontSubsets, value);
            }
        }

        public List<FontSubset> SelectedFontAssets
        {
            get
            {
                List<FontSubset> selectedSubsets = Enum.GetValues(fontSubsets.GetType())
                    .Cast<FontSubset>()
                    .Where(x => fontSubsets.HasFlag(x))
                    .ToList();

                return selectedSubsets;
            }
        }

        private Dictionary<FontSubset, List<FontItem>> googleFontsBySubset = new Dictionary<FontSubset, List<FontItem>>();

        public IEnumerator GetGoogleFontsBySubset(FontSubset fontSubset)
        {
            List<FontSubset> selectedSubsets = Enum.GetValues(fontSubsets.GetType())
                .Cast<FontSubset>()
                .Where(x => fontSubsets.HasFlag(x))
                .ToList();

            List<FontSubset> missingSubsets = new List<FontSubset>();

            foreach (FontSubset subset in Enum.GetValues(fontSubsets.GetType()))
            {
                if (fontSubsets.HasFlag(subset) == false)
                    continue;

                if (googleFontsBySubset.TryGetValue(subset, out var _) == false)
                {
                    missingSubsets.Add(subset);
                }
            }

            if (missingSubsets.Count == 0)
            {
                yield break;
            }

            foreach (FontSubset missingSubset in missingSubsets)
            {
                string missingSubsetName = missingSubset.ToLower();

                if (missingSubsetName == FontSubset.LatinExt.ToLower())
                {
                    missingSubsetName = "latin-ext";
                }


                DALogger.Log(FcuLocKey.loading_google_fonts.Localize(missingSubset.ToString()));

                string gfontsUrl = "https://content-webfonts.googleapis.com/v1/webfonts?subset={0}&key={1}";
                string url = string.Format(gfontsUrl, missingSubsetName, FcuConfig.Instance.GoogleFontsApiKey);

                DARequest request = new DARequest
                {
                    RequestType = RequestType.Get,
                    Query = url
                };

                yield return monoBeh.RequestSender.SendRequest<FontRoot>(request, @return0 =>
                {
                    if (@return0.Success)
                    {
                        googleFontsBySubset.Add(missingSubset, @return0.Object.Items);
                    }
                    else
                    {
                        DALogger.LogError($"Can't get '{missingSubset}' fonts from Google repository.");
                    }
                });
            }
        }

        public string GetUrlByWeight(FontItem fontItem, int weight, FontStyle fontStyle)
        {
            try
            {
                if (fontStyle == FontStyle.Normal)
                {
                    switch (weight)
                    {
                        case 100: return fontItem.Files["100"];
                        case 200: return fontItem.Files["200"];
                        case 300: return fontItem.Files["300"];
                        case 400: return fontItem.Files["regular"];
                        case 500: return fontItem.Files["500"];
                        case 600: return fontItem.Files["600"];
                        case 700: return fontItem.Files["700"];
                        case 800: return fontItem.Files["800"];
                        case 900: return fontItem.Files["900"];
                    }
                }
                else if (fontStyle == FontStyle.Italic)
                {
                    switch (weight)
                    {
                        case 100: return fontItem.Files["100italic"];
                        case 200: return fontItem.Files["200italic"];
                        case 300: return fontItem.Files["300italic"];
                        case 400: return fontItem.Files["italic"];
                        case 500: return fontItem.Files["500italic"];
                        case 600: return fontItem.Files["600italic"];
                        case 700: return fontItem.Files["700italic"];
                        case 800: return fontItem.Files["800italic"];
                        case 900: return fontItem.Files["900italic"];
                    }
                }
            }
            catch
            {

            }

            return null;
        }

        public FontItem GetFontItem(FontMetadata fontMetadata, FontSubset fontSubset)
        {
            try
            {
                googleFontsBySubset.TryGetValue(fontSubset, out var googleFonts);

                foreach (FontItem item in googleFonts)
                {
                    if (item.Family.ToLower() == fontMetadata.Family.ToLower())
                    {
                        return item;
                    }
                }
            }
            catch
            {

            }

            return default;
        }
    }
}
