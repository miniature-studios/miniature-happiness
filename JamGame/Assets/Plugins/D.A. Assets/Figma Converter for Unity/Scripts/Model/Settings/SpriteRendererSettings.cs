using DA_Assets.Shared;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class SpriteRendererSettings : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] bool flipX = false;
        [SerializeField] bool flipY = false;
        [SerializeField] SpriteMaskInteraction maskInteraction = SpriteMaskInteraction.None;
        [SerializeField] SpriteSortPoint sortPoint = SpriteSortPoint.Center;
        [SerializeField] string sortingLayer = "Default";
        [SerializeField] int nextOrderStep = 10;

        public bool FlipX { get => flipX; set => SetValue(ref flipX, value); }
        public bool FlipY { get => flipY; set => SetValue(ref flipY, value); }
        public SpriteMaskInteraction MaskInteraction { get => maskInteraction; set => SetValue(ref maskInteraction, value); }
        public SpriteSortPoint SortPoint { get => sortPoint; set => SetValue(ref sortPoint, value); }
        public string SortingLayer { get => sortingLayer; set => SetValue(ref sortingLayer, value); }

        public int NextOrderStep { get => nextOrderStep; set => SetValue(ref nextOrderStep, value); }
    }
}
