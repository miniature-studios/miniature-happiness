using DA_Assets.DAG;
using DA_Assets.FCU.Extensions;
using DA_Assets.Shared;
using System;
using UnityEngine;
using DA_Assets.Shared.Extensions;

#if DABUTTON_EXISTS
using DA_Assets.DAB;
#endif

#pragma warning disable CS0162

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class DAButtonSettings : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] DAColorBlendMode blendMode = DAColorBlendMode.Overlay;
        public DAColorBlendMode BlendMode { get => blendMode; set => blendMode = value; }

        [SerializeField] float blendIntensity = 1f;
        public float BlendIntensity { get => blendIntensity; set => blendIntensity = value; }

#if DABUTTON_EXISTS
        [SerializeField] DATargetGraphic defaultTargetGraphic;
        [SerializeProperty(nameof(defaultTargetGraphic))]
        public DATargetGraphic DefaultTargetGraphic 
        {
            get
            {
                if (defaultTargetGraphic.IsDefault())
                {
                    defaultTargetGraphic = DAButtonDefaults.Instance.CopyTargetGraphic(DAButtonDefaults.Instance.DefaultTargetGraphic);
                }

                return defaultTargetGraphic;
            }
            set => defaultTargetGraphic = value; 
        }
#endif
    }
}
