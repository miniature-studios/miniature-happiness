using DA_Assets.FCU.Model;
using System;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using DA_Assets.FCU.Extensions;
using UnityEngine;
using UnityEngine.UI;

#if TRUESHADOW_EXISTS
using LeTai.TrueShadow;
#endif

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    [Serializable]
    public class ShadowDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void Draw(FObject fobject)
        {
            switch (monoBeh.Settings.ComponentSettings.ShadowComponent)
            {
                case ShadowComponent.TrueShadow:
                    DrawTrueShadow(fobject);
                    break;
            }
        }

        private void DrawTrueShadow(FObject fobject)
        {
#if TRUESHADOW_EXISTS
            TrueShadow[] oldShadows = fobject.Data.GameObject.GetComponents<TrueShadow>();

            foreach (TrueShadow oldShadow in oldShadows)
                oldShadow.Destroy();

            foreach (Effect effect in fobject.Effects)
            {
                if (effect.Type.ToString().Contains("SHADOW"))
                {
                    monoBeh.Log($"DrawTrueShadow | {fobject.Data.Hierarchy}", FcuLogType.SetTag);

                    Image img = null;

                    if (fobject.ContainsTag(FcuTag.Image) == false)
                    {
                        fobject.Data.GameObject.TryAddGraphic(out img);
                    }

                    fobject.Data.GameObject.TryAddComponent(out TrueShadow trueShadow);

                    if (fobject.ContainsTag(FcuTag.Image) == false)
                    {
                        img.enabled = false;
                    }

                    float x = effect.Offset.x;
                    float y = effect.Offset.y;

                    float angle = Mathf.Atan2(y, x) * (180.0f / Mathf.PI);
                    float distance = Mathf.Sqrt(x * x + y * y);

                    trueShadow.OffsetAngle = angle;
                    trueShadow.OffsetDistance = distance;
                    trueShadow.Spread = effect.Spread.ToFloat();

                    trueShadow.Color = effect.Color;
                    trueShadow.Size = effect.Radius;

                    trueShadow.BlendMode = BlendMode.Multiply;

                    if (effect.Type.ToString().Contains("DROP"))
                        trueShadow.Inset = false;
                    else
                        trueShadow.Inset = true;

                    trueShadow.enabled = true;
                }
            }
#endif
        }
    }
}