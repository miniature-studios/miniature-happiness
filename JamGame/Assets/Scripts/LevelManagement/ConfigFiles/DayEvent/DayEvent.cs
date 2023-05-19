using System;
using UnityEngine;

public abstract class DayEvent : ScriptableObject
{
    public abstract void ShowDayEvent(LevelExecutor level_executor, Action show_end_handler);
}

