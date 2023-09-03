using Common;
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
    public class None : IOverlay
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
        private Gradient gradient;
        public Gradient Gradient => gradient;

        [SerializeField]
        private float minimalStressBound;
        public float MinimalStressBound => minimalStressBound;

        [SerializeField]
        private float maximalStressBound;
        public float MaximalStressBound => maximalStressBound;

        public void Activate(IOverlayManager overlay_manager)
        {
            overlay_manager.ApplyOverlay(this);
        }
    }
}
