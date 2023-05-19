using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SampleDayEvent", menuName = "Level/DayEvent/SampleDayEvent", order = 0)]
public class SampleDayEvent : DayEvent
{
    public override void ShowDayEvent(LevelExecutor level_executor, Action show_end_handler)
    {
        throw new NotImplementedException();
    }
}
