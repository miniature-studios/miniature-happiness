using UnityEngine;
using UnityEngine.UI;

#pragma warning disable CS0414

namespace DA_Assets.Shared
{
    /// <summary>
    /// GridLayoutGroup without fixed CellSize.
    /// <para><see href="https://forum.unity.com/threads/flexible-grid-layout.296074/#post-5368452"/></para>
    /// </summary>
    [ExecuteAlways]
    public class FlexibleGridLayoutGroup : LayoutGroup
    {
        [SerializeField] LayoutAxis _layoutAxis = LayoutAxis.Horizontal;
        [SerializeField, Min(1)] int _constraintCount = 1;
        [SerializeField] Vector2 _spacing = Vector2.zero;
        [SerializeField] bool _childForceExpandWidth = true;
        [SerializeField] bool _childForceExpandHeight = true;
        [SerializeField] bool _childControlWidth = true;
        [SerializeField] bool _childControlHeight = true;
        [SerializeField] bool _childScaleWidth = false;
        [SerializeField] bool _childScaleHeight = false;

        private int _capacity = 8;
        private Vector2[] _sizes = new Vector2[8];

        /// <summary>
        /// Which axis should cells be placed along first
        /// </summary>
        /// <remarks>
        /// When startAxis is set to horizontal, an entire row will be filled out before proceeding to the next row. When set to vertical, an entire column will be filled out before proceeding to the next column.
        /// </remarks>
        public LayoutAxis LayoutAxis
        {
            get
            {
                return _layoutAxis;
            }
            set
            {
                SetProperty(ref _layoutAxis, value);
            }
        }

        /// <summary>
        /// How many cells there should be along the constrained axis.
        /// </summary>
        public int ConstraintCount
        {
            get
            {
                return _constraintCount;
            }
            set
            {
                SetProperty(ref _constraintCount, value);
            }
        }

        /// <summary>
        /// The spacing to use between layout elements in the grid on both axises.
        /// </summary>
        public Vector2 Spacing
        {
            get
            {
                return _spacing;
            }
            set
            {
                SetProperty(ref _spacing, value);
            }
        }

        /// <summary>
        /// Whether to force the children to expand to fill additional available horizontal space.
        /// </summary>
        public bool ChildForceExpandWidth
        {
            get
            {
                return _childForceExpandWidth;
            }
            set
            {
                SetProperty(ref _childForceExpandWidth, value);
            }
        }

        /// <summary>
        /// Whether to force the children to expand to fill additional available vertical space.
        /// </summary>
        public bool ChildForceExpandHeight
        {
            get
            {
                return _childForceExpandHeight;
            }
            set
            {
                SetProperty(ref _childForceExpandHeight, value);
            }
        }

        /// <summary>
        /// Returns true if the Layout Group controls the widths of its children. Returns false if children control their own widths.
        /// </summary>
        /// <remarks>
        /// If set to false, the layout group will only affect the positions of the children while leaving the widths untouched. The widths of the children can be set via the respective RectTransforms in this case.
        ///
        /// If set to true, the widths of the children are automatically driven by the layout group according to their respective minimum, preferred, and flexible widths. This is useful if the widths of the children should change depending on how much space is available.In this case the width of each child cannot be set manually in the RectTransform, but the minimum, preferred and flexible width for each child can be controlled by adding a LayoutElement component to it.
        /// </remarks>
        public bool ChildControlWidth
        {
            get
            {
                return _childControlWidth;
            }
            set
            {
                SetProperty(ref _childControlWidth, value);
            }
        }

        /// <summary>
        /// Returns true if the Layout Group controls the heights of its children. Returns false if children control their own heights.
        /// </summary>
        /// <remarks>
        /// If set to false, the layout group will only affect the positions of the children while leaving the heights untouched. The heights of the children can be set via the respective RectTransforms in this case.
        ///
        /// If set to true, the heights of the children are automatically driven by the layout group according to their respective minimum, preferred, and flexible heights. This is useful if the heights of the children should change depending on how much space is available.In this case the height of each child cannot be set manually in the RectTransform, but the minimum, preferred and flexible height for each child can be controlled by adding a LayoutElement component to it.
        /// </remarks>
        public bool ChildControlHeight
        {
            get
            {
                return _childControlHeight;
            }
            set
            {
                SetProperty(ref _childControlHeight, value);
            }
        }

        /// <summary>
        /// Whether children widths are scaled by their x scale.
        /// </summary>
        public bool ChildScaleWidth
        {
            get
            {
                return _childScaleWidth;
            }
            set
            {
                SetProperty(ref _childScaleWidth, value);
            }
        }

        /// <summary>
        /// Whether children heights are scaled by their y scale.
        /// </summary>
        public bool ChildScaleHeight
        {
            get
            {
                return _childScaleHeight;
            }
            set
            {
                SetProperty(ref _childScaleHeight, value);
            }
        }

        /// <summary>
        /// Called by the layout system to calculate the horizontal layout size.
        /// Also see ILayoutElement
        /// </summary>
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            SetLayoutAlongForAxis(_layoutAxis == LayoutAxis.Vertical, (int)LayoutAxis.Horizontal);
        }

        /// <summary>
        /// Called by the layout system to calculate the vertical layout size.
        /// Also see ILayoutElement
        /// </summary>
        public override void CalculateLayoutInputVertical()
        {
            SetLayoutAlongForAxis(_layoutAxis == LayoutAxis.Vertical, (int)LayoutAxis.Vertical);
        }

        /// <summary>
        /// Called by the layout system
        /// Also see ILayoutElement
        /// </summary>
        public override void SetLayoutHorizontal()
        {
            SetChildrenAlongAxis((int)LayoutAxis.Horizontal, _layoutAxis == LayoutAxis.Vertical);
        }

        /// <summary>
        /// Called by the layout system
        /// Also see ILayoutElement
        /// </summary>
        public override void SetLayoutVertical()
        {
            SetChildrenAlongAxis((int)LayoutAxis.Vertical, _layoutAxis == LayoutAxis.Vertical);
        }

        /// <summary>
        /// Calculate the layout element properties for this layout element along the given axis.
        /// </summary>
        /// <param name="axis">The axis to calculate for. 0 is horizontal and 1 is vertical.</param>
        /// <param name="isVertical">Is this group a vertical group?</param>
        private void CalcAlongAxis(
            bool isVertical,
            int axis,
            int startRectChildernIndex,
            int rectChildrenCount,
            out float totalMin,
            out float totalPreferred,
            out float totalFlexible)
        {
            float spacing = _spacing[axis];
            bool controlSize = axis == 0 ? _childControlWidth : _childControlHeight;
            bool useScale = axis == 0 ? _childScaleWidth : _childScaleHeight;
            bool childForceExpandSize = axis == 0 ? _childForceExpandWidth : _childForceExpandHeight;

            totalMin = 0.0f;// combinedPadding;
            totalPreferred = 0.0f;// combinedPadding;
            totalFlexible = 0;

            bool alongOtherAxis = (isVertical ^ (axis == 1));

            for (int i = 0; i < rectChildrenCount; i++)
            {
                RectTransform child = rectChildren[i + startRectChildernIndex];
                GetChildSizes(child, axis, controlSize, childForceExpandSize, out float min, out float preferred, out float flexible);

                if (useScale)
                {
                    float scaleFactor = child.localScale[axis];
                    min *= scaleFactor;
                    preferred *= scaleFactor;
                    flexible *= scaleFactor;
                }

                if (alongOtherAxis)
                {
                    totalMin = Mathf.Max(min/* + combinedPadding*/, totalMin);
                    totalPreferred = Mathf.Max(preferred/* + combinedPadding*/, totalPreferred);
                    totalFlexible = Mathf.Max(flexible, totalFlexible);
                }
                else
                {
                    totalMin += min + spacing;
                    totalPreferred += preferred + spacing;

                    // Increment flexible size with element's flexible size.
                    totalFlexible += flexible;
                }
            }

            if (!alongOtherAxis && rectChildren.Count > 0)
            {
                totalMin -= spacing;
                totalPreferred -= spacing;
            }

            totalPreferred = Mathf.Max(totalMin, totalPreferred);
        }

        private void SetLayoutAlongForAxis(bool isVertical, int axis)
        {
            bool alongOtherAxis = (isVertical ^ (axis == 1));
            int rectChildrenCount = rectChildren.Count;

            float combinedPadding = (axis == 0 ? padding.horizontal : padding.vertical);
            float spacing = _spacing[axis];
            float totalMin = combinedPadding;
            float totalPreferred = combinedPadding;
            float totalFlexble = 0.0f;

            for (int i = 0; i < rectChildrenCount; i += _constraintCount)
            {
                CalcAlongAxis(isVertical, axis, i, Mathf.Min(rectChildrenCount - i, _constraintCount), out float min, out float preferred, out float flexible);

                if (alongOtherAxis)
                {
                    totalMin += min + spacing;
                    totalPreferred += preferred + spacing;
                    totalFlexble += flexible;
                }
                else
                {
                    totalMin = Mathf.Max(min + combinedPadding, totalMin);
                    totalPreferred = Mathf.Max(preferred + combinedPadding, totalPreferred);
                    totalFlexble = Mathf.Max(flexible, totalFlexble);
                }
            }

            if (alongOtherAxis && rectChildrenCount > 0)
            {
                totalMin -= spacing;
                totalPreferred -= spacing;
            }

            totalPreferred = Mathf.Max(totalMin, totalPreferred);

            SetLayoutInputForAxis(
                totalMin,
                totalPreferred,
                totalFlexble,
                axis);
        }

        /// <summary>
        /// Set the positions and sizes of the child layout elements for the given axis.
        /// </summary>
        /// <param name="axis">The axis to handle. 0 is horizontal and 1 is vertical.</param>
        /// <param name="isVertical">Is this group a vertical group?</param>
        private void SetChildrenAlongAxis(int axis, bool isVertical)
        {
            float size = rectTransform.rect.size[axis];
            float combinedPadding = (axis == 0 ? padding.horizontal : padding.vertical);
            float spacing = _spacing[axis];
            float alignmentOnAxis = GetAlignmentOnAxis(axis);

            bool controlSize = (axis == 0 ? _childControlWidth : _childControlHeight);
            bool useScale = (axis == 0 ? _childScaleWidth : _childScaleHeight);
            bool childForceExpandSize = (axis == 0 ? _childForceExpandWidth : _childForceExpandHeight);

            bool alongOtherAxis = isVertical ^ (axis == 1);

            if (alongOtherAxis)
            {
                float startOffset = 0;
                float itemFlexibleMultiplier = 0.0f, totalMin = GetTotalMinSize(axis), totalPreferred = GetTotalPreferredSize(axis), totalFlexible = GetTotalFlexibleSize(axis);
                float surplusSpace = size - totalPreferred;

                if (surplusSpace > 0.0f)
                {
                    if (totalFlexible > 0.0f)
                        itemFlexibleMultiplier = surplusSpace / totalFlexible;
                    else if (totalFlexible == 0.0f)
                        startOffset = GetStartOffset(axis, totalPreferred - (axis == 0 ? padding.horizontal : padding.vertical)) - (axis == 0 ? padding.left : padding.top);
                }

                float minMaxLerp = 0.0f;

                if (totalMin != totalPreferred)
                    minMaxLerp = Mathf.Clamp01((size - totalMin) / (totalPreferred - totalMin));

                float innerSize = size - combinedPadding, maxSpace = 0.0f;

                for (int i = 0; i < rectChildren.Count; i++)
                {
                    RectTransform child = rectChildren[i];

                    GetChildSizes(child, axis, controlSize, childForceExpandSize, out float min, out float preferred, out float flexible);
                    float scaleFactor = useScale ? child.localScale[axis] : 1f;
                    float requiredSpace = Mathf.Lerp(min, preferred, minMaxLerp);
                    requiredSpace += flexible * itemFlexibleMultiplier;

                    float space = requiredSpace * scaleFactor;

                    if (i % _constraintCount == 0)
                    {
                        if (i != 0)
                            startOffset += maxSpace + spacing;

                        CalcAlongAxis(
                            isVertical,
                            axis,
                            i + 1,
                            Mathf.Min(rectChildren.Count - i, _constraintCount) - 1,
                            out totalMin,
                            out totalPreferred,
                            out totalFlexible);

                        maxSpace = Mathf.Max(space, Mathf.Lerp(totalMin, totalPreferred, minMaxLerp) + totalFlexible * itemFlexibleMultiplier);
                    }

                    float offset = startOffset + GetStartOffset(axis, innerSize - maxSpace + space);

                    if (controlSize)
                    {
                        SetChildAlongAxisWithScale(child, axis, offset, requiredSpace, scaleFactor);
                    }
                    else
                    {
                        float offsetInCell = (requiredSpace - child.sizeDelta[axis]) * alignmentOnAxis;
                        SetChildAlongAxisWithScale(child, axis, offset + offsetInCell, scaleFactor);
                    }
                }
            }
            else
            {/////////////////////////////////////////////////
                float startPos = (axis == 0 ? padding.left : padding.top), pos = startPos, itemFlexibleMultiplier = 0.0f, minMaxLerp = 0.0f;

                for (int i = 0; i < rectChildren.Count; i++)
                {
                    RectTransform child = rectChildren[i];
                    GetChildSizes(child, axis, controlSize, childForceExpandSize, out float min, out float preferred, out float flexible);
                    float scaleFactor = useScale ? child.localScale[axis] : 1f;

                    if (i % _constraintCount == 0)
                    {
                        pos = startPos;

                        CalcAlongAxis(isVertical,
                            axis,
                            i,
                            Mathf.Min(rectChildren.Count - i, _constraintCount),
                            out float totalMin,
                            out float totalPreferred,
                            out float totalFlexible);

                        totalMin += combinedPadding;
                        totalPreferred += combinedPadding;

                        itemFlexibleMultiplier = 0.0f;

                        float surplusSpace = size - totalPreferred;
                        if (surplusSpace > 0.0f)
                        {
                            if (totalFlexible > 0.0f)
                                itemFlexibleMultiplier = surplusSpace / totalFlexible;
                            else if (totalFlexible == 0.0f)
                                pos = GetStartOffset(axis, totalPreferred - (axis == 0 ? padding.horizontal : padding.vertical));
                        }

                        minMaxLerp = 0.0f;
                        if (totalMin != totalPreferred)
                            minMaxLerp = Mathf.Clamp01((size - totalMin) / (totalPreferred - totalMin));
                    }

                    float childSize = Mathf.Lerp(min, preferred, minMaxLerp);
                    childSize += flexible * itemFlexibleMultiplier;

                    if (controlSize)
                    {
                        SetChildAlongAxisWithScale(child, axis, pos, childSize, scaleFactor);
                    }
                    else
                    {
                        float offsetInCell = (childSize - child.sizeDelta[axis]) * alignmentOnAxis;
                        SetChildAlongAxisWithScale(child, axis, pos + offsetInCell, scaleFactor);
                    }

                    pos += childSize * scaleFactor + spacing;
                }
            }
        }

        private void GetChildSizes(
        RectTransform child,
        int axis,
        bool controlSize,
        bool childForceExpand,
        out float min,
        out float preferred,
        out float flexible)
        {
            if (!controlSize)
            {
                min = child.sizeDelta[axis];
                preferred = min;
                flexible = 0;
            }
            else
            {
                min = LayoutUtility.GetMinSize(child, axis);
                preferred = LayoutUtility.GetPreferredSize(child, axis);
                flexible = LayoutUtility.GetFlexibleSize(child, axis);
            }

            if (childForceExpand)
                flexible = Mathf.Max(flexible, 1);
        }

#if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();

            // For new added components we want these to be set to false,
            // so that the user's sizes won't be overwritten before they
            // have a chance to turn these settings off.
            // However, for existing components that were added before this
            // feature was introduced, we want it to be on be default for
            // backwardds compatibility.
            // Hence their default value is on, but we set to off in reset.
            _childControlWidth = false;
            _childControlHeight = false;
        }


        protected virtual void Update()
        {
            if (Application.isPlaying)
                return;

            int count = transform.childCount;

            if (count > _capacity)
            {
                if (count > _capacity * 2)
                    _capacity = count;
                else
                    _capacity *= 2;

                _sizes = new Vector2[_capacity];
            }

            // If children size change in editor, update layout (case 945680 - Child GameObjects in a Horizontal/Vertical Layout Group don't display their correct position in the Editor)
            bool dirty = false;
            for (int i = 0; i < count; i++)
            {
                RectTransform t = transform.GetChild(i) as RectTransform;
                if (t != null && t.sizeDelta != _sizes[0])
                {
                    dirty = true;
                    _sizes[i] = t.sizeDelta;
                }
            }

            if (dirty)
                LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }

#endif

    }

    /// <summary>
    /// The grid axis we are looking at.
    /// </summary>
    /// <remarks>
    /// As the storage is a [][] we make access easier by passing a axis.
    /// </remarks>
    public enum LayoutAxis
    {
        /// <summary>
        /// Horizontal axis
        /// </summary>
        Horizontal = 0,
        /// <summary>
        /// Vertical axis.
        /// </summary>
        Vertical = 1
    }
}