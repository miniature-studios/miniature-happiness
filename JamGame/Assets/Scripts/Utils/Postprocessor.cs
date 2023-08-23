#if UNITY_EDITOR
using Level.Room;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    internal class Postprocessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths,
            bool didDomainReload
        )
        {
            foreach (string str in importedAssets.Concat(movedAssets))
            {
                GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(str);
                if (asset == null)
                {
                    continue;
                }

                CoreModel coreModel = asset.GetComponent<CoreModel>();
                if (coreModel != null)
                {
                    coreModel.SetHashCode();
                }
            }
        }
    }
}
#endif
