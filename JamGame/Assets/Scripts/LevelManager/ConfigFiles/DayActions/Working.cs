using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Working", menuName = "Level/DayActions/Working", order = 2)]
public class Working : DayAction
{
    [SerializeField] private float workingTime;
    public override void ReleaseAction(LevelExecuter LevelExecuter, Action EndActionHandler)
    {
        throw new NotImplementedException();
    }
}

