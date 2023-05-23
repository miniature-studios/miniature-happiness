using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Working", menuName = "Level/DayActions/Working", order = 2)]
public class Working : IDayAction
{
    [SerializeField] private float workingTime;
    public float WorkingTime => workingTime;
    public Action ActionEnd { get; set; }
}

