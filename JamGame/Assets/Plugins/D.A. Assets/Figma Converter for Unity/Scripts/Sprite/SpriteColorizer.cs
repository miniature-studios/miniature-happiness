using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace DA_Assets.FCU
{
    [Serializable]
    public class SpriteColorizer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public IEnumerator ColorizeSprites(List<FObject> fobjects)
        {
            foreach (FObject fobject in fobjects)
            {
                if (fobject.Data.FcuImageType != FcuImageType.Downloadable)
                    continue;

                if (fobject.Data.SingleColor.IsDefault())
                    continue;

                if (File.Exists(fobject.Data.SpritePath.GetFullAssetPath()) == false)
                    continue;

                byte[] rawData = File.ReadAllBytes(fobject.Data.SpritePath.GetFullAssetPath());
                Texture2D tex = new Texture2D(fobject.Data.SpriteSize.x, fobject.Data.SpriteSize.y);
                tex.LoadImage(rawData);

                if (fobject.Data.SingleColor.IsDefault() == false)
                {
                    tex.Colorize(Color.white);

                    byte[] bytes = new byte[0];

                    switch (monoBeh.Settings.MainSettings.ImageFormat)
                    {
                        case ImageFormat.PNG:
                            bytes = tex.EncodeToPNG();
                            break;
                        case ImageFormat.JPG:
                            bytes = tex.EncodeToJPG();
                            break;
                    }

                    File.WriteAllBytes(fobject.Data.SpritePath, bytes);
                }

                yield return null;
            }
        }

        public void SetSingleColors(List<FObject> fobjects)
        {
            Parallel.ForEach(fobjects, fobject =>
            {
                fobject.IsSingleColor(out Color color);
                fobject.Data.SingleColor = color;
            });
        }
    }
}