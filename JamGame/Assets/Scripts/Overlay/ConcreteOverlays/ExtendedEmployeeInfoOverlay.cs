using System;
using UnityEngine;

[Serializable]
public class ExtendedEmployeeInfoOverlay : IOverlay
{
    [SerializeField]
    private GameObject uiPrefab;
    public GameObject UIPrefab => uiPrefab;

    public void Activate(IOverlayManager overlay_manager)
    {
        overlay_manager.ApplyOverlay(this);
    }
}
