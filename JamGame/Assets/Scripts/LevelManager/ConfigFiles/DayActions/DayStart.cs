using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DayStart", menuName = "Level/DayActions/DayStart", order = 0)]
public class DayStart : DayAction
{
    public override void ReleaseAction(LevelExecuter LevelExecuter, Action EndActionHandler)
    {
        throw new NotImplementedException();
    }
}

