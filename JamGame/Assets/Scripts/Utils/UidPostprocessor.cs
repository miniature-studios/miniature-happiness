#if UNITY_EDITOR
using Employee;
using Level.Room;
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
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(str);
                if (asset != null)
                {
                    CoreModel coreModel = asset.GetComponent<CoreModel>();
                    if (coreModel != null)
                    {
                        coreModel.Uid.GenerateIfEmpty();
                        EditorUtility.SetDirty(asset);
                    }
                }

                Buff buff = AssetDatabase.LoadAssetAtPath<Buff>(str);
                if (buff != null)
                {
                    buff.Uid.GenerateIfEmpty();
                    EditorUtility.SetDirty(buff);
                }

                Quirk quirk = AssetDatabase.LoadAssetAtPath<Quirk>(str);
                if (quirk != null)
                {
                    quirk.Uid.GenerateIfEmpty();
                    EditorUtility.SetDirty(quirk);
                }
            }
        }
    }
}
#endif
