using DA_Assets.Shared;
using System;
using UnityEngine;

#pragma warning disable CS0162

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class ButtonSettings : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] Color normalColor = new Color32(255, 255, 255, 255);
        [SerializeProperty(nameof(normalColor))]
        public Color NormalColor => normalColor;

        [SerializeField] Color highlightedColor = new Color32(245, 245, 245, 255);
        [SerializeProperty(nameof(highlightedColor))]
        public Color HighlightedColor => highlightedColor;

        [SerializeField] Color pressedColor = new Color32(200, 200, 200, 255);
        [SerializeProperty(nameof(pressedColor))]
        public Color PressedColor => pressedColor;

        [SerializeField] Color selectedColor = new Color32(245, 245, 245, 255);
        [SerializeProperty(nameof(selectedColor))]
        public Color SelectedColor => selectedColor;

        [SerializeField] Color disabledColor = new Color32(200, 200, 200, 128);
        [SerializeProperty(nameof(disabledColor))]
        public Color DisabledColor => disabledColor;

        [SerializeField] float colorMultiplier = 1;
        [SerializeProperty(nameof(colorMultiplier))]
        public float ColorMultiplier => colorMultiplier;

        [SerializeField] float fadeDuration = 0.1f;
        [SerializeProperty(nameof(fadeDuration))]
        public float FadeDuration => fadeDuration;
    }
}