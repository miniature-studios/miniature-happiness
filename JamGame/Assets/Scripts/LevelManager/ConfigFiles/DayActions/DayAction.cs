using System;
using UnityEngine;

public abstract class DayAction : ScriptableObject
{
    public abstract void ReleaseAction(LevelExecuter LevelExecuter, Action EndActionHandler);
}

