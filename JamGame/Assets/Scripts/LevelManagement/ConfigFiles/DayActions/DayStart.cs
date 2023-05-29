using System;
using UnityEngine;

[Serializable]
public class DayStart : IDayAction
{
    [SerializeField] private int morningMoney;
    public int MorningMoney => morningMoney;
}

