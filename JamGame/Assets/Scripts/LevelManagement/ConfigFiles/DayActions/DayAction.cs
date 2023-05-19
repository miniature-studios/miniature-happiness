using System;
using UnityEngine;

public abstract class DayAction : ScriptableObject
{
    public abstract void ReleaseAction(LevelExecutor Level_executor, Action show_end_handler);
}

