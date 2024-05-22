using System;
using Employee.StressMeter;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Overlay
{
    [HideReferenceObjectPicker]
    public interface IOverlay
    {
        void Activate(IOverlayManager overlay_manager);
    }

    [Serializable]
    public class None : IOverlay
    {
        [FoldoutGroup("None")]
        public string UselessSting;

        public void Activate(IOverlayManager overlay_manager)
        {
            overlay_manager.RevertAllOverlays();
        }
    }

    [Serializable]
    public class ExtendedEmployeeInfo : IOverlay
    {
        [FoldoutGroup("None")]
        public string UselessSting;

        public void Activate(IOverlayManager overlay_manager)
        {
            overlay_manager.ApplyOverlay(this);
        }
    }

    [Serializable]
    public class Stress : IOverlay
    {
        [SerializeField]
        [InlineProperty]
        [HideReferenceObjectPicker]
        [FoldoutGroup("Stress")]
        private Gradient gradient = new();

        [SerializeField]
        [FoldoutGroup("Stress")]
        private float minimalStressBound;
        public float MinimalStressBound => minimalStressBound;

        [SerializeField]
        [FoldoutGroup("Stress")]
        private float maximalStressBound;
        public float MaximalStressBound => maximalStressBound;

        public Color GetCurrentColor(StressMeterImpl stressMeter)
        {
            float normalizedStress = stressMeter.Stress;
            normalizedStress =
                1
                - (
                    (normalizedStress - MinimalStressBound)
                    / (MaximalStressBound - MinimalStressBound)
                );
            return gradient.Evaluate(normalizedStress);
        }

        public void Activate(IOverlayManager overlay_manager)
        {
            overlay_manager.ApplyOverlay(this);
        }
    }
}
