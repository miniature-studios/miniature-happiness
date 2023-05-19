using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "DayEnd", menuName = "Level/DayActions/DayEnd", order = 3)]
public class DayEnd : DayAction
{
    public override void ReleaseAction(LevelExecutor Level_executor, Action show_end_handler)
    {
        throw new NotImplementedException();
    }
}
