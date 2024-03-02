#if UNITY_EDITOR
using System;
using Employee;
using Level.Room;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    internal class UidPostprocessor : AssetPostprocessor
    {
        // TODO: Generalize approach with Uids as it's used in 3 places now:
        // rooms, extended info buffs, extended info quirks.
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

                Buff buff = AssetDatabase.LoadAssetAtPath<Buff>(str);
                if (buff != null && buff.Uid == "")
                {
                    buff.SetHashCode(Guid.NewGuid().ToString());
                    EditorUtility.SetDirty(buff);
                }

                Quirk quirk = AssetDatabase.LoadAssetAtPath<Quirk>(str);
                if (quirk != null && quirk.Uid == "")
                {
                    quirk.SetHashCode(Guid.NewGuid().ToString());
                    EditorUtility.SetDirty(quirk);
                }
            }
        }
    }
}
#endif
