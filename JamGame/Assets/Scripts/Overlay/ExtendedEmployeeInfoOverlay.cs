using System;
using UnityEngine;

[Serializable]
public class ExtendedEmployeeInfoOverlay : IOverlay
{
    [SerializeField] GameObject uiPrefab;
    public GameObject UIPrefab => uiPrefab;

    public void Activate(IOverlayManager overlay_manager)
    {
        overlay_manager.ApplyOverlay(this);
    }
}
