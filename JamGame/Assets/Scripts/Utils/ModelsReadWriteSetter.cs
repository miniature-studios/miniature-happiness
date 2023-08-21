#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    public class ModelsReadWriteFlagSetter
    {
        [MenuItem("Tools/ModelsReadWriteSetter/Set Models read|write flag")]
        private static void Hierarchy()
        {
            IEnumerable<string> files = Directory
                .GetFiles("Assets", "*.*", SearchOption.AllDirectories)
                .Where(x => x.EndsWith("obj") || x.EndsWith("fbx"));

            foreach (string path in files)
            {
                ModelImporter model_importer = (ModelImporter)AssetImporter.GetAtPath(path);
                model_importer.isReadable = true;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }

            Debug.Log("Models read|write flag is set");
        }
    }
}
#endif
