using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
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
        public void Activate(IOverlayManager overlay_manager)
        {
            overlay_manager.RevertAllOverlays();
        }
    }

    [Serializable]
    public class ExtendedEmployeeInfo : IOverlay
    {
        [AssetsOnly]
        [OdinSerialize]
        [FoldoutGroup("Extended Employee Info")]
        public GameObject UIPrefab { get; private set; }

        public void Activate(IOverlayManager overlay_manager)
        {
            overlay_manager.ApplyOverlay(this);
        }
    }

    [Serializable]
    public class Stress : IOverlay
    {
        [OdinSerialize]
        [FoldoutGroup("Stress")]
        public Gradient Gradient { get; private set; }

        [OdinSerialize]
        [FoldoutGroup("Stress")]
        public float MinimalStressBound { get; private set; }

        [OdinSerialize]
        [FoldoutGroup("Stress")]
        public float MaximalStressBound { get; private set; }

        public void Activate(IOverlayManager overlay_manager)
        {
            overlay_manager.ApplyOverlay(this);
        }
    }
}
