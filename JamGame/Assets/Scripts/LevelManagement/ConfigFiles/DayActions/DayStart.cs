using System;
using UnityEngine;

[Serializable]
public class DayStart : IDayAction
{
    [SerializeField] private int morningMoney;
    public int MorningMoney => morningMoney;

    public void Execute(LevelExecutor executor, Action next_action)
    {
        executor.Execute(this, next_action);
    }
}

