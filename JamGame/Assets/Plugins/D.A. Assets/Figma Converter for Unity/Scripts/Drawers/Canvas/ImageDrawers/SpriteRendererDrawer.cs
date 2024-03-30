using DA_Assets.FCU.Extensions;
using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace DA_Assets.FCU.Drawers.CanvasDrawers
{
    public class SpriteRendererDrawer : MonoBehaviourBinder<FigmaConverterUnity>
    {
        public void Draw(FObject fobject, Sprite sprite, GameObject target)
        {
            target.TryAddComponent(out SpriteRenderer sr);
            sr.sprite = sprite;
            sr.sortingOrder = target.transform.GetSiblingIndex();
      
            if (sprite == null)
            {
                sr.sprite = FcuConfig.Instance.SpriteX32;
                sr.drawMode = SpriteDrawMode.Tiled;
                Vector2 size = target.GetComponent<RectTransform>().rect.size;
                sr.size = size;
                SetColor(fobject, sr);
            }
            else
            {
                sr.drawMode = SpriteDrawMode.Simple;

                if (fobject.Data.FcuImageType == FcuImageType.Generative)
                {
                    SetColor(fobject, sr);
                }
                else
                {
                    sr.color = Color.white;
                }
            }

            sr.flipX = monoBeh.Settings.SpriteRendererSettings.FlipX;
            sr.flipY = monoBeh.Settings.SpriteRendererSettings.FlipY;
            sr.maskInteraction = monoBeh.Settings.SpriteRendererSettings.MaskInteraction;
            sr.spriteSortPoint = monoBeh.Settings.SpriteRendererSettings.SortPoint;
            sr.sortingLayerName = monoBeh.Settings.SpriteRendererSettings.SortingLayer;
        }

        public void SetColor(FObject fobject, SpriteRenderer sr)
        {
            FGraphic graphic = fobject.GetGraphic();

            monoBeh.Log($"SetUnityImageColor | {fobject.Data.Hierarchy} | {fobject.Data.FcuImageType} | graphic.HasFills: {graphic.HasFill} | graphic.HasStrokes: {graphic.HasStroke}");

            if (fobject.IsDrawableType())
            {
                if (graphic.HasFill && graphic.HasStroke)
                {
                    // AddUnityOutline(fobject, gr.gameObject, graphic.SolidStroke, graphic.GradientStroke);
                }

                if (graphic.HasFill)
                {
                    if (graphic.SolidFill.IsDefault() == false)
                    {
                        sr.color = graphic.SolidFill.Color;
                    }
                    else
                    {
                        sr.color = Color.white;
                    }

                    if (graphic.GradientFill.IsDefault() == false)
                    {
                        AddGradient(graphic.GradientFill, sr);
                    }
                }
                else if (graphic.HasStroke)
                {
                    if (graphic.SolidStroke.IsDefault() == false)
                    {
                        sr.color = graphic.SolidStroke.Color;
                    }
                    else
                    {
                        sr.color = Color.white;
                    }

                    if (graphic.GradientStroke.IsDefault() == false)
                    {
                        //  AddGradient(graphic.GradientStroke, gr.gameObject);
                    }
                }
                else
                {
                    //    fobject.Data.GameObject.TryDestroyComponent<Outline>();
                }
            }
            else if (fobject.IsGenerativeType())
            {
                if (graphic.HasFill && graphic.HasStroke)//no need colorize
                {
                    if (fobject.StrokeAlign == StrokeAlign.OUTSIDE)
                    {
                        //     AddUnityOutline(fobject, gr.gameObject, graphic.SolidStroke, graphic.GradientStroke);
                    }
                }
                else if (graphic.HasFill)
                {
                    if (graphic.SolidFill.IsDefault() == false)
                    {
                        sr.color = graphic.SolidFill.Color;
                    }
                    else
                    {
                        sr.color = Color.white;
                    }

                    if (graphic.GradientFill.IsDefault() == false)
                    {
                        AddGradient(graphic.GradientFill, sr);
                    }
                }
                else if (graphic.HasStroke)
                {
                    if (graphic.SolidStroke.IsDefault() == false)
                    {
                        sr.color = graphic.SolidStroke.Color;
                    }
                    else
                    {
                        sr.color = Color.white;
                    }

                    if (graphic.GradientStroke.IsDefault() == false)
                    {
                        //   AddGradient(graphic.GradientStroke, gr.gameObject);
                    }
                }
            }
            else if (fobject.IsDownloadableType())
            {
                if (fobject.Data.SingleColor.IsDefault() == false)
                {
                    sr.color = fobject.Data.SingleColor;
                }
                else
                {
                    sr.color = Color.white;
                }
            }
        }

        public void AddGradient(Paint gradientColor, SpriteRenderer sr)
        {
            List<GradientColorKey> gradientColorKeys = gradientColor.ToGradientColorKeys();
            List<GradientAlphaKey> gradientAlphaKeys = gradientColor.ToGradientAlphaKeys();

            if (!gradientColorKeys.IsEmpty() && !gradientAlphaKeys.IsEmpty())
            {
                sr.color = ImageExtensions.SetFigmaAlpha(gradientColorKeys[0].color, gradientAlphaKeys[0].alpha);
            }
        }
    }
}