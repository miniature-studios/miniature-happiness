using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Utils
{
    public class ModelsReadWriteSetterTools
    {
        [MenuItem("Tools/ModelsReadWriteSetter/Set Models Read|Write flag in Assets on true")]
        private static void Hierarchy()
        {
            IEnumerable<string> files = Directory
                .GetFiles("Assets", "*.*", SearchOption.AllDirectories)
                .Where(x => x.EndsWith("obj") || x.EndsWith("fbx"));

            foreach (string path in files)
            {
                ModelImporter A = (ModelImporter)AssetImporter.GetAtPath(path);
                A.isReadable = true;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }

            Debug.Log("Hierarchy animators setup done");
        }
    }
}
