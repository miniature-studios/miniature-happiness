#if UNITY_EDITOR
using Common;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    internal class UidPostprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths,
            bool didDomainReload
        )
        {
            foreach (string str in importedAssets)
            {
                Object asset = AssetDatabase.LoadAssetAtPath<Object>(str);
                if (asset != null)
                {
                    if (asset is IPostprocessedUidHandle uidHandle)
                    {
                        uidHandle.Uid.GenerateIfEmpty();
                        EditorUtility.SetDirty(asset);
                    }

                    if (
                        asset is GameObject gameObject
                        && gameObject.GetComponent<IPostprocessedUidHandle>() != null
                    )
                    {
                        gameObject.GetComponent<IPostprocessedUidHandle>().Uid.GenerateIfEmpty();
                        EditorUtility.SetDirty(asset);
                    }
                }
            }
        }
    }
}
#endif
