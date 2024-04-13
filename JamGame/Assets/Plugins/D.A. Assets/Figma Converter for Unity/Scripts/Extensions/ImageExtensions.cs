using DA_Assets.FCU.Model;
using DA_Assets.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU.Extensions
{
    public static class ImageExtensions
    {
        private static int roundDigits = 3;

        public static bool GetBoundingSize(this FObject fobject, out Vector2 size)
        {
            size = default(Vector2);

            float? x = fobject.AbsoluteBoundingBox.Width;
            float? y = fobject.AbsoluteBoundingBox.Height;

            if (x == null || y == null)
            {
                return false;
            }

            float xR = (float)Math.Round(x.Value, roundDigits);
            float yR = (float)Math.Round(y.Value, roundDigits);

            size = new Vector2(xR, yR);
            return true;
        }

        public static bool GetBoundingPosition(this FObject fobject, out Vector2 position)
        {
            position = default(Vector2);

            float? x = fobject.AbsoluteBoundingBox.X;
            float? y = fobject.AbsoluteBoundingBox.Y;

            if (x == null || y == null)
            {
                return false;
            }

            float xR = (float)Math.Round(x.Value, roundDigits);
            float yR = (float)Math.Round(y.Value, roundDigits);

            position = new Vector2(xR, yR);
            return true;
        }

        public static bool GetRenderSize(this FObject fobject, out Vector2 size)
        {
            size = default(Vector2);

            float? x = fobject.AbsoluteRenderBounds.Width;
            float? y = fobject.AbsoluteRenderBounds.Height;

            if (x == null || y == null)
            {
                return false;
            }

            float xR = (float)Math.Round(x.Value, roundDigits);
            float yR = (float)Math.Round(y.Value, roundDigits);

            size = new Vector2(xR, yR);
            return true;
        }

        public static bool GetRenderPosition(this FObject fobject, out Vector2 position)
        {
            position = default(Vector2);

            float? x = fobject.AbsoluteRenderBounds.X;
            float? y = fobject.AbsoluteRenderBounds.Y;

            if (x == null || y == null)
            {
                return false;
            }

            float xR = (float)Math.Round(x.Value, roundDigits);
            float yR = (float)Math.Round(y.Value, roundDigits);

            position = new Vector2(xR, yR);
            return true;
        }

        public static Vector4 GetCornerRadius(this FObject fobject, ImageComponent imageComponent, bool uitk = false)
        {
            if (fobject.CornerRadiuses.IsEmpty())
            {
                return new Vector4
                {
                    x = fobject.CornerRadius.ToFloat(),
                    y = fobject.CornerRadius.ToFloat(),
                    z = fobject.CornerRadius.ToFloat(),
                    w = fobject.CornerRadius.ToFloat()
                };
            }
            else
            {
                if (uitk)
                {
                    return new Vector4
                    {

                        x = fobject.CornerRadiuses[0],
                        y = fobject.CornerRadiuses[3],
                        z = fobject.CornerRadiuses[2],
                        w = fobject.CornerRadiuses[1]
                    };
                }
                else if (imageComponent == ImageComponent.ProceduralImage)
                {
                    return new Vector4
                    {
                        x = fobject.CornerRadiuses[0],
                        y = fobject.CornerRadiuses[1],
                        z = fobject.CornerRadiuses[2],
                        w = fobject.CornerRadiuses[3]
                    };
                }
                else
                {
                    return new Vector4
                    {
                        x = fobject.CornerRadiuses[3],
                        y = fobject.CornerRadiuses[2],
                        z = fobject.CornerRadiuses[1],
                        w = fobject.CornerRadiuses[0]
                    };
                }
            }
        }
        
        public static bool IsZeroSize(this FObject fobject)
        {
            if (fobject.AbsoluteBoundingBox.Width == 0 || fobject.AbsoluteBoundingBox.Height == 0)
            {
                return true;
            }

            return false;
        }

        public static bool IsVisible(this FObject fobject)
        {
            if (fobject.Visible != null && fobject.Visible == false)
                return false;

            return true;
        }

        public static bool IsVisible(this Paint paint)
        {
            if (paint.Visible != null && paint.Visible == false)
                return false;

            return true;
        }

        public static bool IsVisible(this Effect effect)
        {
            if (effect.Visible != null && effect.Visible == false)
                return false;

            return true;
        }

        public static bool IsSingleColor(this FObject fobject, out Color color)
        {
            Dictionary<Color, float?> values = new Dictionary<Color, float?>();
            List<bool> flags = new List<bool>();

            IsSingleColorRecursive(fobject, flags, values);

            if (flags.Count > 0)
            {
                color = default;
                return false;
            }

            if (values.Count == 1)
            {
                color = values.First().Key;
                return true;
            }
            else
            {
                color = default;
                return false;
            }
        }

        public static bool HasImageOrGifRef(this FObject fobject)
        {
            if (fobject.Fills.IsEmpty())
                return false;

            foreach (Paint item in fobject.Fills)
            {
                if (item.Visible.ToBoolNullTrue() == false)
                    continue;

                if (item.ImageRef.IsEmpty() == false || item.GifRef.IsEmpty() == false)
                    return true;
            }

            return false;
        }

        private static void IsSingleColorRecursive(FObject fobject, List<bool> flags, Dictionary<Color, float?> values)
        {
            if (fobject.Fills.IsEmpty() == false)
            {
                foreach (var item in fobject.Fills)
                {
                    if (item.ImageRef.IsEmpty() == false || item.GifRef.IsEmpty() == false)
                    {
                        flags.Add(true);
                        return;
                    }

                    if (item.Type.ToString().Contains("SOLID") == false)
                    {
                        flags.Add(true);
                        return;
                    }

                    if (item.IsVisible())
                    {
                        values.TryAddValue<Color, float?>(item.Color, item.Opacity);
                    }
                }
            }

            if (fobject.Strokes.IsEmpty() == false)
            {
                foreach (var item in fobject.Strokes)
                {
                    if (item.ImageRef.IsEmpty() == false || item.GifRef.IsEmpty() == false)
                    {
                        flags.Add(true);
                        return;
                    }

                    if (item.Type.ToString().Contains("SOLID") == false)
                    {
                        flags.Add(true);
                        return;
                    }

                    if (item.IsVisible())
                    {
                        values.TryAddValue<Color, float?>(item.Color, item.Opacity);
                    }
                }
            }

            if (fobject.Effects.IsEmpty() == false)
            {
                foreach (var item in fobject.Effects)
                {
                    if (item.Type.ToString().Contains("SOLID") == false)
                    {
                        flags.Add(true);
                        return;
                    }

                    if (item.IsVisible())
                    {
                        values.TryAddValue<Color, float?>(item.Color, item.Opacity);
                    }
                }
            }

            if (fobject.Children.IsEmpty())
                return;

            foreach (var item in fobject.Children)
            {
                if (item.ContainsTag(FcuTag.Text))
                    continue;

                IsSingleColorRecursive(item, flags, values);
            }
        }

        public static bool ContainsRoundedCorners(this FObject fobject)
        {
            return fobject.CornerRadius > 0 || (fobject.CornerRadiuses?.Any(radius => radius > 0)).ToBoolNullFalse();
        }

        public static bool IsArcDataFilled(this FObject fobject)
        {
            if (fobject.ArcData.Equals(default(ArcData)))
            {
                return false;
            }

            return fobject.ArcData.EndingAngle < 6.28f;
        }

        public static bool IsGradient(this Paint paint)
        {
            return paint.Type.ToString().Contains("GRADIENT");
        }

        public static bool TryGetFirstGradient(this FObject fobject, out Paint gradient)
        {
            if (fobject.Fills.IsEmpty())
            {
                gradient = default;
                return false;
            }

            foreach (Paint _fill in fobject.Fills)
            {
                if (_fill.Visible == false)
                    continue;
            }

            gradient = default;
            return false;
        }

        public static FGraphic GetGraphic(this FObject fobject)
        {
            bool hasFills = fobject.Fills.TryGetColors(out Paint solidFill, out Paint gradientFill);
            bool hasStroke = fobject.Strokes.TryGetColors(out Paint solidStroke, out Paint gradientStroke);

            FGraphic graphic = new FGraphic
            {
                HasFill = hasFills,
                HasStroke = hasStroke,

                SolidFill = solidFill,
                SolidStroke = solidStroke,

                GradientFill = gradientFill,
                GradientStroke = gradientStroke,
            };

            if (fobject.StrokeWeight <= 0)
            {
                graphic.HasStroke = false;
                graphic.SolidStroke = default;
                graphic.GradientStroke = default;
            }

            return graphic;
        }

        public static Color SetFigmaAlpha(Color color, float? opacity)
        {
            return new Color(color.r, color.g, color.b, opacity == null ? 1 : opacity.ToFloat());
        }

        public static bool TryGetColors(this List<Paint> paints, out Paint solidFill, out Paint gradientFill)
        {


            Paint _solidFill = default;
            Paint _gradientFill = default;

            foreach (Paint _fill in paints)
            {
                if (_fill.IsVisible() == false)
                    continue;

                if (_fill.Type.ToString().Contains("SOLID"))
                {
                    if (_solidFill.IsDefault())
                    {
                        _solidFill = _fill;
                        _solidFill.Color = SetFigmaAlpha(_solidFill.Color, _fill.Opacity);
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (_fill.Type.ToString().Contains("GRADIENT"))
                {
                    if (_gradientFill.IsDefault())
                    {
                        if (_fill.Type.ToString().Contains("LINEAR"))
                        {
                            _gradientFill = _fill;
                            _gradientFill.Color = SetFigmaAlpha(_gradientFill.Color, _fill.Opacity);
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
            }

            solidFill = _solidFill;
            gradientFill = _gradientFill;

            if (solidFill.IsDefault() && gradientFill.IsDefault())
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}