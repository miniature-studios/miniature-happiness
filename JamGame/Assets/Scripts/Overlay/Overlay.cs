using System;
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
        [AssetsOnly]
        [SerializeField]
        [FoldoutGroup("Extended Employee Info")]
        private GameObject uiPrefab;
        public GameObject UIPrefab => uiPrefab;

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
        public Gradient Gradient => gradient;

        [SerializeField]
        [FoldoutGroup("Stress")]
        private float minimalStressBound;
        public float MinimalStressBound => minimalStressBound;

        [SerializeField]
        [FoldoutGroup("Stress")]
        private float maximalStressBound;
        public float MaximalStressBound => maximalStressBound;

        public void Activate(IOverlayManager overlay_manager)
        {
            overlay_manager.ApplyOverlay(this);
        }
    }
}
