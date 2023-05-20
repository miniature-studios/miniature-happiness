using UnityEngine;

public class StressOverlay : MonoBehaviour, IOverlay
{
    [SerializeField] private Color minimalStressColor;
    [SerializeField] private float minimalStressBound;

    public Color MinimalStressColor => minimalStressColor;
    public float MinimalStressBound => minimalStressBound;

    [SerializeField] private Color maximalStressColor;
    [SerializeField] private float maximalStressBound;

    public Color MaximalStressColor => maximalStressColor;
    public float MaximalStressBound => maximalStressBound;

    public void Activate(Location location)
    {
        location.ApplyOverlay(this);
    }
}
