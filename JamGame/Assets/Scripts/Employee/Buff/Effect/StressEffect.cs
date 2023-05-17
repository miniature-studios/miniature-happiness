using System;
using UnityEngine;

[Serializable]
public class StressEffect : IEffect
{
    [SerializeField] private float increaseMultiplier;
    public float IncreaseMultiplier => increaseMultiplier;
}
