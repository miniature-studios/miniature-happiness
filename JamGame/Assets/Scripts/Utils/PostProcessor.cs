using Common;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    internal class MyAllPostprocessor : AssetPostprocessor
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
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(str);
                if (asset == null)
                {
                    continue;
                }

                IUniqueIdHandler idHandler = asset.GetComponent<IUniqueIdHandler>();
                if (idHandler != null)
                {
                    if (idHandler.CoreModel == null)
                    {
                        Debug.LogError($"No core model link in {str}", asset);
                    }
                    else
                    {
                        idHandler.UniqueId = idHandler.CoreModel.UniqueId;
                    }
                }
            }
        }
    }
}
