using UnityEngine;
using System;
using DA_Assets.Shared;

#if TextMeshPro
using TMPro;
#endif

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class TextMeshSettings : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] bool autoSize = false;
        [SerializeField] bool overrideTags = false;
        [SerializeField] bool wrapping = true;
        [SerializeField] bool richText = true;
        [SerializeField] bool raycastTarget = true;
        [SerializeField] bool parseEscapeCharacters = true;
        [SerializeField] bool visibleDescender = true;
        [SerializeField] bool kerning = true;
        [SerializeField] bool extraPadding = false;
#if TextMeshPro
        [SerializeField] TextOverflowModes overflow = TextOverflowModes.Overflow;
        [SerializeField] TextureMappingOptions horizontalMapping = TextureMappingOptions.Character;
        [SerializeField] TextureMappingOptions verticalMapping = TextureMappingOptions.Character;
        [SerializeField] VertexSortingOrder geometrySorting = VertexSortingOrder.Normal;
#endif
        [SerializeField] Shader shader;

        public bool AutoSize { get => autoSize; set => SetValue(ref autoSize, value); }
        public bool OverrideTags { get => overrideTags; set => SetValue(ref overrideTags, value); }
        public bool Wrapping { get => wrapping; set => SetValue(ref wrapping, value); }
        public bool RichText { get => richText; set => SetValue(ref richText, value); }
        public bool RaycastTarget { get => raycastTarget; set => SetValue(ref raycastTarget, value); }
        public bool ParseEscapeCharacters { get => parseEscapeCharacters; set => SetValue(ref parseEscapeCharacters, value); }
        public bool VisibleDescender { get => visibleDescender; set => SetValue(ref visibleDescender, value); }
        public bool Kerning { get => kerning; set => SetValue(ref kerning, value); }
        public bool ExtraPadding { get => extraPadding; set => SetValue(ref extraPadding, value); }
#if TextMeshPro
        public TextOverflowModes Overflow { get => overflow; set => SetValue(ref overflow, value); }
        public TextureMappingOptions HorizontalMapping { get => horizontalMapping; set => SetValue(ref horizontalMapping, value); }
        public TextureMappingOptions VerticalMapping { get => verticalMapping; set => SetValue(ref verticalMapping, value); }
        public VertexSortingOrder GeometrySorting { get => geometrySorting; set => SetValue(ref geometrySorting, value); }
#endif
        public Shader Shader { get => shader; set => SetValue(ref shader, value); }

    }

    [Flags]
    public enum FontSubset
    {
        Latin = 1 << 0,
        LatinExt = 1 << 1,
        Sinhala = 1 << 2,
        Greek = 1 << 3,
        Hebrew = 1 << 4,
        Vietnamese = 1 << 5,
        Cyrillic = 1 << 6,
        CyrillicExt = 1 << 7,
        Devanagari = 1 << 8,
        Arabic = 1 << 9,
        Khmer = 1 << 10,
        Tamil = 1 << 11,
        GreekExt = 1 << 12,
        Thai = 1 << 13,
        Bengali = 1 << 14,
        Gujarati = 1 << 15,
        Oriya = 1 << 16,
        Malayalam = 1 << 17,
        Gurmukhi = 1 << 18,
        Kannada = 1 << 19,
        Telugu = 1 << 20,
        Myanmar = 1 << 21,
    }
}