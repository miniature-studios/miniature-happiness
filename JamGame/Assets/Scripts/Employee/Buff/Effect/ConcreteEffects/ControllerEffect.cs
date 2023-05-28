using System;
using UnityEngine;

[Serializable]
public class ControllerEffect : IEffect
{
    [SerializeField] private float speedMultiplier;
    public float SpeedMultiplier => speedMultiplier;
}
