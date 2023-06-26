using System;
using UnityEngine;

namespace Overlay
{
    [InterfaceEditor]
    public interface IOverlay
    {
        void Activate(IOverlayManager overlay_manager);
    }

    [Serializable]
    public class No : IOverlay
    {
        public void Activate(IOverlayManager overlay_manager)
        {
            overlay_manager.RevertAllOverlays();
        }
    }

    [Serializable]
    public class ExtendedEmployeeInfo : IOverlay
    {
        [SerializeField]
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
        private Color minimalStressColor;

        [SerializeField]
        private float minimalStressBound;

        public Color MinimalStressColor => minimalStressColor;
        public float MinimalStressBound => minimalStressBound;

        [SerializeField]
        private Color maximalStressColor;

        [SerializeField]
        private float maximalStressBound;

        public Color MaximalStressColor => maximalStressColor;
        public float MaximalStressBound => maximalStressBound;

        public void Activate(IOverlayManager overlay_manager)
        {
            overlay_manager.ApplyOverlay(this);
        }
    }
}
