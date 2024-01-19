#if UNITY_EDITOR
using System.IO;
using SettingsConfigs;
using TileUnion;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    internal class ProjectedTilesValidator
    {
        [MenuItem("Tools/ProjectedTilesValidator/Validate Projected Tiles in TileUnions")]
        private static void OnPostprocessAllAssets()
        {
            string[] files = Directory.GetFiles("Assets", "*.prefab", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                bool dirty = false;
                GameObject prefab = PrefabUtility.LoadPrefabContents(file);
                if (prefab != null)
                {
                    TileUnionImpl tileUnion = prefab.GetComponent<TileUnionImpl>();
                    if (tileUnion != null)
                    {
                        tileUnion.ValidateProjectedTiles(
                            GlobalGameSettings.GetProjectedTilesSettings().ProjectedCount
                        );
                        dirty = true;
                    }
                }
                if (dirty)
                {
                    _ = PrefabUtility.SaveAsPrefabAsset(prefab, file);
                }
                PrefabUtility.UnloadPrefabContents(prefab);
            }
        }
    }
}
#endif
