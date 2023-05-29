using System;
using UnityEngine;

[Serializable]
public class Working : IDayAction
{
    [SerializeField] private float workingTime;
    public float WorkingTime => workingTime;
}

