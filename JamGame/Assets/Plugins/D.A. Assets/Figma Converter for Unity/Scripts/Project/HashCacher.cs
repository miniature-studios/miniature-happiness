using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class HashCacher : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void WriteCache(List<FObject> fobjects)
        {
            string projectFilePath = $"{monoBeh.Settings.MainSettings.ProjectUrl.ReplaceInvalidFileNameChars()}.txt";
            string fullPath = Path.Combine(ProjectCacher.GetCachePath("HashCache"), projectFilePath);

            FHash fHash = new FHash
            {
                Hashes = fobjects.ConvertAll(fobject => new FHashObject
                {
                    Id = $"{fobject.Data.Parent.Id}_{fobject.Data.Id}",
                    HashData = fobject.Data.HashData,
                    HashDataTree = fobject.Data.HashDataTree
                }).ToArray()
            };

            string json = JsonUtility.ToJson(fHash);
            Path.GetDirectoryName(fullPath).CreateFolderIfNotExists();
            File.WriteAllText(fullPath, json);
        }

        public void LoadCache(SyncHelper[] syncHelpers)
        {
            foreach (SyncHelper syncHelper in syncHelpers)
            {
                string projectFilePath = $"{monoBeh.Settings.MainSettings.ProjectUrl.ReplaceInvalidFileNameChars()}.txt";
                string fullPath = Path.Combine(ProjectCacher.GetCachePath("HashCache"), projectFilePath);

                if (File.Exists(fullPath))
                {
                    string json = File.ReadAllText(fullPath);
                    FHash fHash = JsonUtility.FromJson<FHash>(json);

                    foreach (FHashObject hashObject in fHash.Hashes)
                    {
                        if (hashObject.Id == $"{syncHelper.Data.Parent.Id}_{syncHelper.Data.Id}")
                        {
                            syncHelper.Data.HashData = hashObject.HashData;
                            syncHelper.Data.HashDataTree = hashObject.HashDataTree;
                            break;
                        }
                    }
                }
            }
        }

        [System.Serializable]
        private struct FHash
        {
            public FHashObject[] Hashes;
        }

        [System.Serializable]
        private struct FHashObject
        {
            public string Id;
            public string HashData;
            public string HashDataTree;
        }
    }
}
