using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

#pragma warning disable CS0162

namespace DA_Assets.FCU
{
    [Serializable]
    public class SpriteWorker : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public Sprite GetSprite(FObject fobject)
        {
            if (fobject.Data.SpritePath.IsEmpty())
            {
                return null;
            }
#if UNITY_EDITOR
            Sprite sprite = (Sprite)UnityEditor.AssetDatabase.LoadAssetAtPath(fobject.Data.SpritePath, typeof(Sprite));
            return sprite;
#endif
            return null;
        }

        public IEnumerator MarkAsSprites(List<FObject> fobjects)
        {
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();

            List<FObject> fobjectWithSprite = fobjects.Where(x => x.Data.SpritePath != null).ToList();

            int allCount = fobjectWithSprite.Count();
            int count = 0;

            DACycles.ForEach(fobjectWithSprite, fobject =>
            {
                SetImgTypeSprite(fobject, () =>
                {
                    count++;
                }).StartDARoutine(monoBeh);
            }, WaitFor.Delay001().WaitTimeF, 100).StartDARoutine(monoBeh);

            int tempCount = -1;
            while (FcuLogger.WriteLogBeforeEqual(
                ref count,
                ref allCount,
                FcuLocKey.log_mark_as_sprite.Localize(count, allCount),
                ref tempCount))
            {
                yield return WaitFor.Delay1();
            }
#endif
            yield return null;
        }
#if UNITY_EDITOR
        public IEnumerator SetImgTypeSprite(FObject fobject, Action callback)
        {
            while (true)
            {
                bool success = SetTextureSettings(fobject);

                if (success)
                {
                    callback.Invoke();
                    break;
                }

                yield return WaitFor.Delay01();
            }
        }


        private bool SetTextureSettings(FObject fobject)
        {
            try
            {
                UnityEditor.TextureImporter importer = UnityEditor.AssetImporter.GetAtPath(fobject.Data.SpritePath) as UnityEditor.TextureImporter;
                SetTextureSize(fobject, importer);

                if (IsTextureSettingsCorrect(importer))
                {
                    return true;
                }
                else
                {
                    UpdateTextureSettings(importer, fobject.Data.SpritePath);
                    return false;
                }
            }
            catch
            {
                //DALogger.Log(FcuLocKey.cant_load_sprite.Localize(fobject.Data.SpritePath, ex.Message));
                return true;
            }
        }

        private void SetTextureSize(FObject fobject, UnityEditor.TextureImporter importer)
        {
            importer.GetTextureSize(out int width, out int height);
            importer.SetMaxTextureSize(width, height);
            fobject.Data.SpriteSize = new Vector2Int(width, height);
        }

        private bool IsTextureSettingsCorrect(UnityEditor.TextureImporter importer)
        {
            bool part1 = importer.isReadable == TextureImporterSettings.IsReadable &&
                   importer.textureType == TextureImporterSettings.TextureType &&
                   importer.crunchedCompression == TextureImporterSettings.CrunchedCompression &&
                   importer.textureCompression == TextureImporterSettings.TextureCompression &&
                   importer.mipmapEnabled == TextureImporterSettings.MipmapEnabled &&
                   importer.spriteImportMode == TextureImporterSettings.SpriteImportMode;

            bool perUnit;

            if (monoBeh.UsingSpriteRenderer())
            {
                perUnit = importer.spritePixelsPerUnit == monoBeh.Settings.MainSettings.ImageScale;
            }
            else
            {
                perUnit = importer.spritePixelsPerUnit == monoBeh.Settings.MainSettings.PixelsPerUnit;
            }

            return part1 && perUnit;
        }

        private void UpdateTextureSettings(UnityEditor.TextureImporter importer, string spritePath)
        {
            importer.isReadable = TextureImporterSettings.IsReadable;
            importer.textureType = TextureImporterSettings.TextureType;
            importer.crunchedCompression = TextureImporterSettings.CrunchedCompression;
            importer.textureCompression = TextureImporterSettings.TextureCompression;
            importer.mipmapEnabled = TextureImporterSettings.MipmapEnabled;
            importer.spriteImportMode = TextureImporterSettings.SpriteImportMode;

            if (monoBeh.UsingSpriteRenderer())
            {
                importer.spritePixelsPerUnit = monoBeh.Settings.MainSettings.ImageScale;
            }
            else
            {
                importer.spritePixelsPerUnit = monoBeh.Settings.MainSettings.PixelsPerUnit;
            }

            if (importer.crunchedCompression)
            {
                importer.compressionQuality = TextureImporterSettings.CompressionQuality;
            }

            UnityEditor.AssetDatabase.WriteImportSettingsIfDirty(spritePath);
            UnityEditor.AssetDatabase.Refresh();
        }

        class TextureImporterSettings
        {
            public static bool IsReadable => true;
            public static UnityEditor.TextureImporterType TextureType => UnityEditor.TextureImporterType.Sprite;
            public static bool CrunchedCompression => FcuConfig.Instance.CrunchedCompression;
            public static int CompressionQuality => FcuConfig.Instance.CrunchedCompressionQuality;
            public static UnityEditor.TextureImporterCompression TextureCompression => FcuConfig.Instance.TextureImporterCompression;
            public static bool MipmapEnabled => FcuConfig.Instance.GenerateMipMaps;
            public static UnityEditor.SpriteImportMode SpriteImportMode => UnityEditor.SpriteImportMode.Single;
        }
#endif
    }
}