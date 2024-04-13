using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DA_Assets.Shared.Extensions;
using DA_Assets.FCU.Extensions;
using System.Reflection;

#pragma warning disable CS0649

#if MPUIKIT_EXISTS
using MPUIKIT;
#endif


namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    public class MPUIKitDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void Draw(FObject fobject, Sprite sprite, GameObject target)
        {
#if MPUIKIT_EXISTS
            target.TryAddGraphic(out MPImage img);
            SetCorners(fobject, img);

            SetColor(fobject, img);

            img.sprite = sprite;
            img.type = monoBeh.Settings.MPUIKitSettings.Type;
            img.raycastTarget = monoBeh.Settings.MPUIKitSettings.RaycastTarget;
            img.preserveAspect = monoBeh.Settings.MPUIKitSettings.PreserveAspect;
            img.FalloffDistance = monoBeh.Settings.MPUIKitSettings.FalloffDistance;
#if UNITY_2020_1_OR_NEWER
            img.raycastPadding = monoBeh.Settings.MPUIKitSettings.RaycastPadding;
#endif

            MethodInfo initMethod = typeof(MPImage).GetMethod("Init", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            initMethod.Invoke(img, null);
#endif
        }

#if MPUIKIT_EXISTS
        public void SetColor(FObject fobject, MPImage img)
        {
            FGraphic graphic = fobject.GetGraphic();

            monoBeh.Log($"SetUnityImageColor | {fobject.Data.Hierarchy} | {fobject.Data.FcuImageType} | hasFills: {graphic.HasFill} | hasStroke: {graphic.HasStroke}");

            img.GradientEffect = new GradientEffect
            {
                Enabled = false,
                GradientType = MPUIKIT.GradientType.Linear,
                Gradient = null
            };

            if (fobject.IsDrawableType())
            {
                if (graphic.HasFill)
                {
                    if (graphic.SolidFill.IsDefault() == false)
                    {
                        Color c = graphic.SolidFill.Color;
                        img.color = c;
                    }
                    else
                    {
                        Color c = Color.white;
                        img.color = c;
                    }

                    if (graphic.GradientFill.IsDefault() == false)
                    {
                        AddGradient(graphic.GradientFill, img);
                    }
                }
                else
                {
                    Color c = Color.white;
                    c.a = 0;
                    img.color = c;
                }

                if (graphic.HasStroke)
                {
                    if (fobject.StrokeAlign == StrokeAlign.INSIDE)
                    {
                        img.OutlineWidth = fobject.StrokeWeight;

                        if (graphic.SolidStroke.IsDefault() == false)
                        {
                            img.OutlineColor = graphic.SolidStroke.Color;
                        }
                        else if (graphic.GradientStroke.IsDefault() == false)
                        {
                            List<GradientColorKey> gradientColorKeys = graphic.GradientStroke.ToGradientColorKeys();
                            img.OutlineColor = gradientColorKeys.First().color;
                        }
                        else
                        {
                            img.OutlineColor = default;
                        }
                    }
                    else if (fobject.StrokeAlign == StrokeAlign.OUTSIDE)
                    {
                        monoBeh.CanvasDrawer.ImageDrawer.UnityImageDrawer.AddUnityOutline(fobject, img.gameObject, graphic.SolidStroke, graphic.GradientStroke);
                    }
                    else
                    {
                        img.OutlineWidth = 0;
                    }
                }
                else
                {
                    img.OutlineWidth = 0;
                }
            }
            else
            {
                monoBeh.CanvasDrawer.ImageDrawer.UnityImageDrawer.SetColor(fobject, img);
            }
        }

        public void AddGradient(Paint gradientColor, MPImage img)
        {
            Gradient gradient = new Gradient
            {
                mode = GradientMode.Blend,
            };

            img.GradientEffect = new GradientEffect
            {
                Enabled = true,
                GradientType = MPUIKIT.GradientType.Linear,
                Gradient = gradient,
                Rotation = gradientColor.GradientHandlePositions.ToAngle()
            };

            List<GradientColorKey> gradientColorKeys = gradientColor.ToGradientColorKeys();
            gradient.colorKeys = gradientColorKeys.ToArray();
        }

        private void SetCorners(FObject fobject, MPImage img)
        {
            if (fobject.Type == NodeType.ELLIPSE)
            {
                img.DrawShape = DrawShape.Circle;
                img.Circle = new Circle
                {
                    FitToRect = true
                };
            }
            else
            {
                img.DrawShape = DrawShape.Rectangle;

                img.Rectangle = new Rectangle
                {
                    CornerRadius = fobject.GetCornerRadius(ImageComponent.MPImage)
                };
            }
        }
#endif
    }
}
