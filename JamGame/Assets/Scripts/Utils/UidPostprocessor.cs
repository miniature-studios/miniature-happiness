#if UNITY_EDITOR
using Employee;
using Employee.Personality;
using Level.Room;
using UnityEditor;

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
                CoreModel coreModel = AssetDatabase.LoadAssetAtPath<CoreModel>(str);
                if (coreModel != null)
                {
                    coreModel.Uid.GenerateIfEmpty();
                    EditorUtility.SetDirty(coreModel);
                    continue;
                }

                Buff buff = AssetDatabase.LoadAssetAtPath<Buff>(str);
                if (buff != null)
                {
                    buff.Uid.GenerateIfEmpty();
                    EditorUtility.SetDirty(buff);
                    continue;
                }

                Quirk quirk = AssetDatabase.LoadAssetAtPath<Quirk>(str);
                if (quirk != null)
                {
                    quirk.Uid.GenerateIfEmpty();
                    EditorUtility.SetDirty(quirk);
                    continue;
                }
            }
        }
    }
}
#endif
