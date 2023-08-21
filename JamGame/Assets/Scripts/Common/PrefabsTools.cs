using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Common
{
    public static class PrefabsTools
    {
        public static IEnumerable<GameObject> GetAllAssetsPrefabs()
        {
            foreach (string guid in AssetDatabase.FindAssets("t:prefab", new string[] { "Assets" }))
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (AssetDatabase.LoadAssetAtPath<GameObject>(path) is GameObject asset)
                {
                    yield return asset;
                }
            }
        }
    }
}
