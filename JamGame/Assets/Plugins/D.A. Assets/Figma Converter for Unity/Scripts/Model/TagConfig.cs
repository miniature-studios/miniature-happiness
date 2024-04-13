using System;
using UnityEngine;

#pragma warning disable CS0649

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public struct TagConfig
    {
        [SerializeField] string label;
        [SerializeField] FcuTag fcuTag;
        [SerializeField] string figmaTag;
        [SerializeField] bool customButtonTag;
        [SerializeField] bool canBeDownloaded;
        [SerializeField] bool canBeInsideSingleImage;
        [SerializeField] bool hasComponent;

        public FcuTag FcuTag => fcuTag;
        public string FigmaTag => figmaTag;
        public bool CustomButtonTag => customButtonTag;
        public bool CanBeDownloaded => canBeDownloaded;
        /// <summary>
        /// If the parent component includes at least one component with 1 == fall, it cannot be a downloadable image.
        /// </summary>
        public bool CanBeInsideSingleImage => canBeInsideSingleImage;
        /// <summary>
        /// Needed to count instantiated scripts/components.
        /// </summary>
        public bool HasComponent => hasComponent;

        public void SetDefaultData(FcuTag fcuTag)
        {
            this.label = fcuTag.ToString();
            this.fcuTag = fcuTag;
        }
    }
}