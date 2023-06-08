using System;
using UnityEngine;

[Serializable]
public class Working : IDayAction
{
    [SerializeField]
    private float workingTime;
    public float WorkingTime => workingTime;

    public void Execute(LevelExecutor executor, Action next_action)
    {
        executor.Execute(this, next_action);
    }
}
