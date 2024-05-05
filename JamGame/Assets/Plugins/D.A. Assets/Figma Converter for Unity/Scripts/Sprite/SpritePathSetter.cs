using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class SpritePathSetter : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public IEnumerator SetSpritePaths(List<FObject> fobjects)
        {
#if UNITY_EDITOR
            List<FObject> canHasFile = fobjects
                .Where(x => x.IsDownloadableType() || x.IsGenerativeType())
                .ToList();

            yield return SetSpriteNames(canHasFile);

            List<FObject> noDuplicates = canHasFile
                .GroupBy(x => x.Data.Hash)
                .Select(x => x.First())
                .ToList();

            yield return null;

            string filter = $"t:{typeof(Sprite).Name}";

            string[] searchInFolder = new string[]
            {
                monoBeh.Settings.MainSettings.SpritesPath
            };

            string[] assetSpritePathes = UnityEditor.AssetDatabase
                .FindAssets(filter, searchInFolder)
                .Select(x => UnityEditor.AssetDatabase.GUIDToAssetPath(x))
                .ToArray();

            yield return DACycles.ForEach(noDuplicates, item =>
            {
                bool imageFileExists = GetSpritePath(assetSpritePathes, item, out string spritePath);

                SetNeedDownloadFileFlag(item, imageFileExists);
                SetNeedGenerateFlag(item, imageFileExists);

                foreach (FObject fobject in fobjects)
                {
                    if (fobject.Data.Hash == item.Data.Hash)
                    {
                        if (imageFileExists)
                        {
                            fobject.Data.SpritePath = spritePath;
                        }
                        else
                        {
                            fobject.Data.SpritePath = GetSpritePath(item);
                        }

                        fobject.Data.SpritePath = fobject.Data.SpritePath.ToUnitySeparators();
                    }
                }
            }, WaitFor.Delay001().WaitTimeF, 500);
#endif
            yield return null;
        }

        private string GetSpritePath(FObject fobject)
        {
            string spriteDir;

            if (fobject.Data.IsMutual)
                spriteDir = "Mutual";
            else
                spriteDir = fobject.Data.RootFrame.NewName;

            string absoluteFramePath = Path.Combine(monoBeh.Settings.MainSettings.SpritesPath.GetFullAssetPath(), spriteDir);
            string relativeAssetPath = Path.Combine(monoBeh.Settings.MainSettings.SpritesPath, spriteDir, fobject.Data.SpriteName);
            Directory.CreateDirectory(absoluteFramePath);

            return relativeAssetPath;
        }

        public IEnumerator SetSpriteNames(List<FObject> fobjects)
        {
            for (int i = 0; i < fobjects.Count(); i++)
            {
                string name = "";

                name += fobjects[i].Data.NewName;
                name += " ";
                name += $"{monoBeh.Settings.MainSettings.ImageScale.ToDotString()}x";
                name += " ";
                name += fobjects[i].Data.Hash;
                name += $".{monoBeh.Settings.MainSettings.ImageFormat.ToLower()}";

                fobjects[i].Data.SpriteName = name;
            }

            yield return null;
        }

        public bool GetSpritePath(string[] spritePathes, FObject fobject, out string path)
        {
            foreach (string spritePath in spritePathes)
            {
                bool get1 = spritePath.TryParseSpriteName(out float scale1, out System.Numerics.BigInteger hash1);
                bool get2 = fobject.Data.SpriteName.TryParseSpriteName(out float scale2, out System.Numerics.BigInteger hash2);

                if (get1 && get2)
                {
                    if (hash1 == hash2)
                    {
                        path = spritePath;
                        return true;
                    }
                }
            }

            path = null;
            return false;
        }

        private void SetNeedDownloadFileFlag(FObject fobject, bool imageFileExists)
        {
            if (fobject.IsDownloadableType()/* || fobject.IsGenerativeType()*/)
            {
                if (monoBeh.Settings.MainSettings.RedownloadSprites)
                {
                    fobject.Data.NeedDownload = true;
                }
                else if (imageFileExists)
                {
                    fobject.Data.NeedDownload = false;
                }
                else
                {
                    fobject.Data.NeedDownload = true;
                }
            }
            else
            {
                fobject.Data.NeedDownload = false;
            }
        }
        private void SetNeedGenerateFlag(FObject fobject, bool imageFileExists)
        {
            if (fobject.IsGenerativeType())
            {
                if (monoBeh.Settings.MainSettings.RedownloadSprites)
                {
                    fobject.Data.NeedGenerate = true;
                }
                else if (imageFileExists)
                {
                    fobject.Data.NeedGenerate = false;
                }
                else
                {
                    fobject.Data.NeedGenerate = true;
                }
            }
            else
            {
                fobject.Data.NeedGenerate = false;
            }
        }
    }
}
