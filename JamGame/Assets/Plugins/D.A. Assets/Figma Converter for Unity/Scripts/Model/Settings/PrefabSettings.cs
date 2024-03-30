using DA_Assets.Shared;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class PrefabSettings : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] TextPrefabNameType textPrefabNameType = TextPrefabNameType.HumanizedColorString;
        public TextPrefabNameType TextPrefabNameType { get => textPrefabNameType; set => SetValue(ref textPrefabNameType, value); }
    }

    public enum TextPrefabNameType
    {
        HumanizedColorString,
        HumanizedColorHEX,
        Figma,
    }
}
