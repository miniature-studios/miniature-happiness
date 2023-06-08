using System;
using UnityEngine;

[Serializable]
public class StressOverlay : IOverlay
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
