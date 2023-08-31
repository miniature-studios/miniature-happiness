#if UNITY_EDITOR
using Level.Room;
using System;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    internal class RoomCoreModelsPostprocessor : AssetPostprocessor
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
                if (asset != null)
                {
                    CoreModel coreModel = asset.GetComponent<CoreModel>();
                    if (coreModel != null && coreModel.Uid == "")
                    {
                        coreModel.SetHashCode(Guid.NewGuid().ToString());
                        EditorUtility.SetDirty(asset);
                    }
                }
            }
        }
    }
}
#endif
