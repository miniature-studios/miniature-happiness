using System;
using UnityEngine;

public abstract class DayEvent : ScriptableObject
{
    public abstract void ShowDayEvent(LevelExecuter LevelExecuter, Action ShowEndHendler);
}

