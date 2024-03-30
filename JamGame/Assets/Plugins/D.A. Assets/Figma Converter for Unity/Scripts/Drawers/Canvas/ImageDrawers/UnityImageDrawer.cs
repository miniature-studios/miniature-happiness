using DA_Assets.DAG;
using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    public class UnityImageDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        private bool TryAddCornerRounder(FObject fobject, GameObject target)
        {
            if (fobject.Size.IsSupportedRenderSize(monoBeh, out Vector2Int _, out Vector2Int __))
            {
                return false;
            }

            Vector4 cr = fobject.GetCornerRadius(ImageComponent.UnityImage);

            if (cr.IsDefault() == false)
            {
                target.TryAddComponent(out CornerRounder cornerRounder);
                cornerRounder.SetRadii(cr);
            }

            return false;
        }

        public void Draw(FObject fobject, Sprite sprite, GameObject target)
        {
            MaskableGraphic graphic;

            if (monoBeh.UsingRawImage())
            {
                target.TryAddGraphic(out RawImage img);
                graphic = img;

                img.texture = sprite.texture;
            }
            else
            {
                target.TryAddGraphic(out Image img);
                graphic = img;

                img.sprite = sprite;
                img.type = monoBeh.Settings.UnityImageSettings.Type;
                img.preserveAspect = monoBeh.Settings.UnityImageSettings.PreserveAspect;
            }

            graphic.raycastTarget = monoBeh.Settings.UnityImageSettings.RaycastTarget;
            graphic.maskable = monoBeh.Settings.UnityImageSettings.Maskable;
#if UNITY_2020_1_OR_NEWER
            graphic.raycastPadding = monoBeh.Settings.UnityImageSettings.RaycastPadding;
#endif

            SetColor(fobject, graphic);
            TryAddCornerRounder(fobject, target);
        }

        public void SetColor(FObject fobject, MaskableGraphic gr)
        {
            FGraphic graphic = fobject.GetGraphic();

            monoBeh.Log($"SetUnityImageColor | {fobject.Data.Hierarchy} | {fobject.Data.FcuImageType} | graphic.HasFills: {graphic.HasFill} | graphic.HasStrokes: {graphic.HasStroke}");

            if (fobject.IsDrawableType())
            {
                if (graphic.HasFill && graphic.HasStroke)
                {
                    AddUnityOutline(fobject, gr.gameObject, graphic.SolidStroke, graphic.GradientStroke);
                }

                if (graphic.HasFill)
                {
                    if (graphic.SolidFill.IsDefault() == false)
                    {
                        gr.color = graphic.SolidFill.Color;
                    }
                    else
                    {
                        gr.color = Color.white;
                    }

                    if (graphic.GradientFill.IsDefault() == false)
                    {
                        AddGradient(graphic.GradientFill, gr.gameObject);
                    }
                }
                else if (graphic.HasStroke)
                {
                    if (graphic.SolidStroke.IsDefault() == false)
                    {
                        gr.color = graphic.SolidStroke.Color;
                    }
                    else
                    {
                        gr.color = Color.white;
                    }

                    if (graphic.GradientStroke.IsDefault() == false)
                    {
                        AddGradient(graphic.GradientStroke, gr.gameObject);
                    }
                }
                else
                {
                    fobject.Data.GameObject.TryDestroyComponent<Outline>();
                }
            }
            else if (fobject.IsGenerativeType())
            {
                if (graphic.HasFill && graphic.HasStroke)//no need colorize
                {
                    if (fobject.StrokeAlign == StrokeAlign.OUTSIDE)
                    {
                        AddUnityOutline(fobject, gr.gameObject, graphic.SolidStroke, graphic.GradientStroke);
                    }
                }
                else if (graphic.HasFill)
                {
                    if (graphic.SolidFill.IsDefault() == false)
                    {
                        gr.color = graphic.SolidFill.Color;
                    }
                    else
                    {
                        gr.color = Color.white;
                    }

                    if (graphic.GradientFill.IsDefault() == false)
                    {
                        AddGradient(graphic.GradientFill, gr.gameObject);
                    }
                }
                else if (graphic.HasStroke)
                {
                    if (graphic.SolidStroke.IsDefault() == false)
                    {
                        gr.color = graphic.SolidStroke.Color;
                    }
                    else
                    {
                        gr.color = Color.white;
                    }

                    if (graphic.GradientStroke.IsDefault() == false)
                    {
                        AddGradient(graphic.GradientStroke, gr.gameObject);
                    }
                }
            }
            else if (fobject.IsDownloadableType())
            {
                if (fobject.Data.SingleColor.IsDefault() == false)
                {
                    gr.color = fobject.Data.SingleColor;
                }
                else
                {
                    gr.color = Color.white;
                }
            }
        }

        public void AddUnityOutline(FObject fobject, GameObject target, Paint solidStroke, Paint gradientStroke)
        {
            if (monoBeh.UsingUnityImage() == false)
            {
                if (fobject.StrokeAlign == StrokeAlign.INSIDE)
                {
                    return;
                }
            }

            target.TryAddComponent(out Outline outline);
            outline.effectDistance = new Vector2(fobject.StrokeWeight, -fobject.StrokeWeight);

            if (solidStroke.IsDefault() == false)
            {
                outline.effectColor = solidStroke.Color;
            }
            else if (gradientStroke.IsDefault() == false)
            {
                List<GradientColorKey> gradientColorKeys = gradientStroke.ToGradientColorKeys();
                outline.effectColor = gradientColorKeys.First().color;
            }
            else
            {
                outline.effectColor = default;
            }
        }

        public void AddGradient(Paint gradientColor, GameObject go)
        {
            if (monoBeh.UsingMPUIKit())
                return;

            go.TryAddComponent(out DAGradient gradient);

            List<GradientColorKey> gradientColorKeys = gradientColor.ToGradientColorKeys();
            List<GradientAlphaKey> gradientAlphaKeys = gradientColor.ToGradientAlphaKeys();

            gradient.BlendMode = DAColorBlendMode.Multiply;
            gradient.Gradient.colorKeys = gradientColorKeys.ToArray();
            gradient.Gradient.alphaKeys = gradientAlphaKeys.ToArray();
            gradient.Angle = gradientColor.GradientHandlePositions.ToAngle();
        }
    }
}