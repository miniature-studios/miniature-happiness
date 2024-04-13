using DA_Assets.FCU.Model;
using DA_Assets.Shared;
using DA_Assets.Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DA_Assets.FCU.Extensions
{
    public static class AutoLayoutExtensions
    {
        public static int GetColumnCount(this FObject fobject)
        {
            HashSet<float> centersY = new HashSet<float>();

            //Finding the centers of the elements along the Y axis to get the number of lines in the Grid.
            foreach (FObject child in fobject.Children)
            {
                float centerY = child.AbsoluteBoundingBox.Y.ToFloat() + (child.AbsoluteBoundingBox.Height.ToFloat() / 2);
                centersY.TryAddValue(centerY);
            }

            int linesCount = centersY.Count;

            int columnCount = fobject.Children.Count / linesCount;
            return columnCount;
        }

        public static RectOffset GetPadding(this FObject fobject)
        {
            try
            {
                return new RectOffset
                {
                    bottom = (int)Mathf.Round(fobject.PaddingBottom.ToFloat()),
                    top = (int)Mathf.Round(fobject.PaddingTop.ToFloat()),
                    left = (int)Mathf.Round(fobject.PaddingLeft.ToFloat()),
                    right = (int)Mathf.Round(fobject.PaddingRight.ToFloat())
                };
            }
            catch (System.Exception ex)
            {
                DALogger.LogError(ex.Message);
                return new RectOffset(0, 0, 0, 0);
            }
        }

        public static TextAnchor GetHorLayoutAnchor(this FObject fobject)
        {
            string aligment = "";
            aligment += fobject.PrimaryAxisAlignItems;
            aligment += " ";
            aligment += fobject.CounterAxisAlignItems;

            switch (aligment)
            {
                case "NONE NONE":
                    return TextAnchor.UpperLeft;
                case "SPACE_BETWEEN NONE":
                    return TextAnchor.UpperCenter;
                case "CENTER NONE":
                    return TextAnchor.UpperCenter;
                case "MAX NONE":
                    return TextAnchor.UpperRight;
                case "NONE CENTER":
                    return TextAnchor.MiddleLeft;
                case "NONE BASELINE":
                    return TextAnchor.MiddleLeft;
                case "SPACE_BETWEEN CENTER":
                    return TextAnchor.MiddleCenter;
                case "CENTER CENTER":
                    return TextAnchor.MiddleCenter;
                case "CENTER BASELINE":
                    return TextAnchor.MiddleCenter;
                case "MAX CENTER":
                    return TextAnchor.MiddleRight;
                case "MAX BASELINE":
                    return TextAnchor.MiddleRight;
                case "NONE MAX":
                    return TextAnchor.LowerLeft;
                case "SPACE_BETWEEN MAX":
                    return TextAnchor.LowerCenter;
                case "CENTER MAX":
                    return TextAnchor.LowerCenter;
                case "MAX MAX":
                    return TextAnchor.LowerRight;
            }

            DALogger.LogError(FcuLocKey.log_unknown_aligment.Localize(aligment, fobject.Data.Hierarchy));
            return TextAnchor.UpperLeft;
        }

        public static TextAnchor GetVertLayoutAnchor(this FObject fobject)
        {
            string aligment = "";
            aligment += fobject.PrimaryAxisAlignItems;
            aligment += " ";
            aligment += fobject.CounterAxisAlignItems;

            switch (aligment)
            {
                case "NONE NONE":
                    return TextAnchor.UpperLeft;
                case "NONE CENTER":
                    return TextAnchor.UpperCenter;
                case "NONE MAX":
                    return TextAnchor.UpperRight;
                case "CENTER NONE":
                    return TextAnchor.MiddleLeft;
                case "SPACE_BETWEEN NONE":
                    return TextAnchor.MiddleLeft;
                case "CENTER CENTER":
                    return TextAnchor.MiddleCenter;
                case "SPACE_BETWEEN CENTER":
                    return TextAnchor.MiddleCenter;
                case "CENTER MAX":
                    return TextAnchor.MiddleRight;
                case "SPACE_BETWEEN MAX":
                    return TextAnchor.MiddleRight;
                case "MAX NONE":
                    return TextAnchor.LowerLeft;
                case "MAX CENTER":
                    return TextAnchor.LowerCenter;
                case "MAX MAX":
                    return TextAnchor.LowerRight;
            }

            DALogger.LogError(FcuLocKey.log_unknown_aligment.Localize(aligment, fobject.Data.Hierarchy));
            return TextAnchor.UpperLeft;
        }

        public static bool IsNeedStretchByX(this FObject fobject)
        {
            HashSet<float?> layoutGrows = new HashSet<float?>();

            foreach (FObject child in fobject.Children)
            {
                layoutGrows.Add(child.LayoutGrow);
            }

            if (layoutGrows.Count == 1)
            {
                if (layoutGrows.First() == 1)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsNeedStretchByY(this FObject fobject)
        {
            HashSet<string> layoutAligns = new HashSet<string>();

            foreach (FObject child in fobject.Children)
            {
                layoutAligns.Add(child.LayoutAlign);
            }

            if (layoutAligns.Count == 1)
            {
                if (layoutAligns.First() == "STRETCH")
                {
                    return true;
                }
            }

            return false;
        }

        public static float GetHorSpacing(this FObject fobject)
        {
            if (fobject.PrimaryAxisAlignItems == PrimaryAxisAlignItem.SPACE_BETWEEN)
            {
                if (fobject.Data.ChildIndexes.IsEmpty())
                {
                    return 0;
                }
                else if (fobject.Data.ChildIndexes.Count == 1)
                {
                    return 0;
                }
                else
                {
                    int childCount = fobject.Data.ChildIndexes.Count;
                    int spacingCount = childCount - 1;
                    float parentWidth = fobject.Data.Size.x;

                    float allChildsWidth = 0;

                    foreach (FObject child in fobject.Children)
                    {
                        allChildsWidth += child.Data.Size.x;
                    }

                    float spacingWidth = (parentWidth - allChildsWidth) / spacingCount;
                    return spacingWidth;
                }
            }
            else
            {
                return fobject.ItemSpacing.ToFloat();
            }
        }
        public static float GetVertSpacing(this FObject fobject)
        {
            if (fobject.PrimaryAxisAlignItems == PrimaryAxisAlignItem.SPACE_BETWEEN)
            {
                if (fobject.Data.ChildIndexes.IsEmpty())
                {
                    return 0;
                }
                else if (fobject.Data.ChildIndexes.Count == 1)
                {
                    return 0;
                }
                else
                {
                    int childCount = fobject.Data.ChildIndexes.Count;
                    int spacingCount = childCount - 1;
                    float parentHeight = fobject.Data.Size.y;

                    float allChildsHeight = 0;

                    foreach (FObject child in fobject.Children)
                    {
                        allChildsHeight += child.Data.Size.y;
                    }

                    float spacingWidth = (parentHeight - allChildsHeight) / spacingCount;
                    return spacingWidth;
                }
            }
            else
            {
                return fobject.ItemSpacing.ToFloat();
            }
        }
    }
}
