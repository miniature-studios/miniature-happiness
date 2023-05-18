using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DayEnd", menuName = "Level/DayActions/DayEnd", order = 3)]
public class DayEnd : DayAction
{
    public override void ReleaseAction(LevelExecuter LevelExecuter, Action EndActionHandler)
    {
        throw new NotImplementedException();
    }
}
