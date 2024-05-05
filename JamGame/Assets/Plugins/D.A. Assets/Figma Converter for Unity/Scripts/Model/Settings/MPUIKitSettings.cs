using DA_Assets.Shared;
using System;
using UnityEngine;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class MPUIKitSettings : MonoBehaviourBinder<FigmaConverterUnity>
    {
        [SerializeField] UnityEngine.UI.Image.Type type = UnityEngine.UI.Image.Type.Simple;
        public UnityEngine.UI.Image.Type Type { get => type; set => SetValue(ref type, value); }

        [SerializeField] bool raycastTarget = true;
        public bool RaycastTarget { get => raycastTarget; set => SetValue(ref raycastTarget, value); }

        [SerializeField] bool preserveAspect = true;
        public bool PreserveAspect { get => preserveAspect; set => SetValue(ref preserveAspect, value); }

        [SerializeField] Vector4 raycastPadding = new Vector4(0, 0, 0, 0);
        public Vector4 RaycastPadding { get => raycastPadding; set => SetValue(ref raycastPadding, value); }

        ////////////////////////////////////////////////////////

        [SerializeField] float falloffDistance = 0.5f;
        public float FalloffDistance { get => falloffDistance; set => SetValue(ref falloffDistance, value); }
    }
}