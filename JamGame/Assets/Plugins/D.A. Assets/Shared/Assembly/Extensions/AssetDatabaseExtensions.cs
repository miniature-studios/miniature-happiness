using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DA_Assets.Shared.Extensions
{
    public static class AssetDatabaseExtensions
    {
        public static List<string> FindAssetsPathes<T>(this string path, string customType = null)
        {
            List<string> assetPathes = new List<string>();

            if (customType == null)
                customType = $"t:{typeof(T).Name}";

#if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets(customType, new string[] { path.ToRelativePath() });
            assetPathes = guids.Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x)).ToList();
#endif
            return assetPathes;
        }

        public static IEnumerator LoadAssetFromFolder<T>(this string fontsPath, Action<List<T>> assets, string customType = null) where T : UnityEngine.Object
        {
            List<string> pathes = new List<string>();
            List<T> loadedAssets = new List<T>();

            if (customType == null)
                customType = $"t:{typeof(T).Name}";

#if UNITY_EDITOR
            string[] guids = UnityEditor.AssetDatabase.FindAssets(customType, new string[] { fontsPath.ToRelativePath() });

            pathes = guids.Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x)).ToList();

            yield return DACycles.ForEach(pathes, path =>
            {
                T sourceFontFile = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
                if (sourceFontFile.IsDefault() == false)
                {
                    loadedAssets.Add(sourceFontFile);
                }
            }, WaitFor.Delay001().WaitTimeF, 25);
#endif

            assets.Invoke(loadedAssets);

            yield return null;
        }
    }
}