using DA_Assets.Shared;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class UnityTextSettings : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] float fontLineSpacing = 1.0f;
        [SerializeField] HorizontalWrapMode horizontalWrapMode = HorizontalWrapMode.Wrap;
        [SerializeField] VerticalWrapMode verticalWrapMode = VerticalWrapMode.Truncate;
        [SerializeField] bool bestFit = true;
        public float FontLineSpacing { get => fontLineSpacing; set => SetValue(ref fontLineSpacing, value); }
        public HorizontalWrapMode HorizontalWrapMode { get => horizontalWrapMode; set => SetValue(ref horizontalWrapMode, value); }
        public VerticalWrapMode VerticalWrapMode { get => verticalWrapMode; set => SetValue(ref verticalWrapMode, value); }
        public bool BestFit { get => bestFit; set => SetValue(ref bestFit, value); }
    }
}
