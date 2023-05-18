using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SampleDayEvent", menuName = "Level/DayEvent/SampleDayEvent", order = 0)]
public class SampleDayEvent : DayEvent
{
    public override void ShowDayEvent(LevelExecuter LevelExecuter, Action ShowEndHendler)
    {
        throw new NotImplementedException();
    }
}
