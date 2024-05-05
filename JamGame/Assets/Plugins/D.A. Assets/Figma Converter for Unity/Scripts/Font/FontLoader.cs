using DA_Assets.Shared;
using DA_Assets.FCU.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DA_Assets.Shared.Extensions;
using DA_Assets.FCU.Extensions;

#if TextMeshPro
using TMPro;
#endif

namespace DA_Assets.FCU
{
    [Serializable]
    public class FontLoader : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] string ttfFontsPath = "Assets/Fonts/Ttf";
        public string TtfFontsPath { get => ttfFontsPath; set => SetValue(ref ttfFontsPath, value); }

        [SerializeField] string tmpFontsPath = "Assets/Fonts/Sdf";
        public string TmpFontsPath { get => tmpFontsPath; set => SetValue(ref tmpFontsPath, value); }

        [SerializeField] List<Font> ttfFonts = new List<Font>();
        [SerializeProperty(nameof(ttfFonts))]
        public List<Font> TtfFonts { get => ttfFonts; set => ttfFonts = value; }

#if TextMeshPro
        [SerializeField] List<TMP_FontAsset> tmpFonts = new List<TMP_FontAsset>();
        [SerializeProperty(nameof(tmpFonts))]
        public List<TMP_FontAsset> TmpFonts { get => tmpFonts; set => tmpFonts = value; }
#endif

        public void RemoveDubAndMissingFonts()
        {
            monoBeh.FontLoader.TtfFonts = monoBeh.FontLoader.TtfFonts.Where(x => x != null).ToList();
            monoBeh.FontLoader.TtfFonts = monoBeh.FontLoader.TtfFonts.Distinct().ToList();
#if TextMeshPro
            monoBeh.FontLoader.TmpFonts = monoBeh.FontLoader.TmpFonts.Where(x => x != null).ToList();
            monoBeh.FontLoader.TmpFonts = monoBeh.FontLoader.TmpFonts.Distinct().ToList();
#endif
        }

        public T GetFontFromArray<T>(FObject fobject, List<T> fontArray) where T : UnityEngine.Object
        {
            fobject.Data.HasFontAsset = true;

            T fontItem = null;

            //Search by family, weight and italic
            foreach (T _fontItem in fontArray)
            {
                if (_fontItem.IsDefault())
                    continue;

                if (fobject.FontNameToString(true, true, null, true) == _fontItem.name.FormatFontName())
                {
                    fontItem = _fontItem;
                    break;
                }
            }

            //If font not found, search by family and weight
            if (fontItem == null)
            {
                foreach (T _fontItem in fontArray)
                {
                    if (_fontItem.IsDefault())
                        continue;

                    if (fobject.FontNameToString(true, false, null, true) == _fontItem.name.FormatFontName())
                    {
                        fontItem = _fontItem;
                        break;
                    }
                }
            }

            //If font not found, search by family only
            if (fontItem == null)
            {
                fobject.Data.HasFontAsset = false;

                foreach (T _fontItem in fontArray)
                {
                    if (_fontItem.IsDefault())
                        continue;

                    string fontFamily = fobject.FontNameToString(false, false, null, true);

                    if (_fontItem.name.FormatFontName().Contains(fontFamily))
                    {
                        fontItem = _fontItem;
                        break;
                    }
                }
            }

            //If font not found, load default font.
            if (fontItem == null)
            {
                fobject.Data.HasFontAsset = false;

                if (typeof(T) == typeof(Font))
                {
#if UNITY_2022_1_OR_NEWER
                    fontItem = Resources.GetBuiltinResource<T>("LegacyRuntime.ttf");
#else
                    fontItem = Resources.GetBuiltinResource<T>("Arial.ttf");
#endif

                }
                else
                {
                    fontItem = Resources.Load<T>("Fonts & Materials/LiberationSans SDF");
                }
            }

            return fontItem;
        }

        public IEnumerator AddToTtfFontsList()
        {
            DALogger.Log(FcuLocKey.log_start_adding_to_fonts_list.Localize());

            yield return AddToList(
                monoBeh.FontLoader.TtfFontsPath,
                monoBeh.FontLoader.TtfFonts,
                addedCount =>
                {
                    DALogger.Log(FcuLocKey.log_added_total.Localize(addedCount, monoBeh.FontLoader.TtfFonts.Count()));
                });

            RemoveDubAndMissingFonts();
        }
        public IEnumerator AddToTmpMeshFontsList()
        {
#if TextMeshPro
            DALogger.Log(FcuLocKey.log_start_adding_to_fonts_list.Localize());

            yield return AddToList(
                monoBeh.FontLoader.TmpFontsPath,
                monoBeh.FontLoader.TmpFonts,
                addedCount =>
                {
                    DALogger.Log(FcuLocKey.log_added_total.Localize(addedCount, monoBeh.FontLoader.TmpFonts.Count()));
                }).StartDARoutine(monoBeh);

            RemoveDubAndMissingFonts();
#endif
            yield return null;
        }

        public IEnumerator AddToList<T>(string fontsPath, List<T> list, Action<int> addedCount) where T : UnityEngine.Object
        {
            List<T> loadedAssets = new List<T>();
            yield return LoadAssetFromFolder<T>(fontsPath, x => loadedAssets = x);

            int count = 0;

            yield return DACycles.ForEach(loadedAssets, asset =>
            {
                if (list.Contains(asset) == false)
                {
                    count++;
                    list.Add(asset);
                }
            }, WaitFor.Delay001().WaitTimeF, 25);

            addedCount.Invoke(count);
        }

        public IEnumerator LoadAssetFromFolder<T>(string fontsPath, Action<List<T>> assets) where T : UnityEngine.Object
        {
            List<string> pathes = new List<string>();
            List<T> loadedAssets = new List<T>();

#if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets($"t:{typeof(T).Name}", new string[] { fontsPath.ToRelativePath() });
            pathes = guids.Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x)).ToList();

            yield return DACycles.ForEach(pathes, path =>
            {
                T sourceFontFile = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                loadedAssets.Add(sourceFontFile);
            }, WaitFor.Delay001().WaitTimeF, 25);
#endif

            assets.Invoke(loadedAssets);
            yield return null;
        }
    }
}
