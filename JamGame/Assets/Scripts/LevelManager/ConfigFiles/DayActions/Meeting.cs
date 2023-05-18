using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "Meeting", menuName = "Level/DayActions/Meeting", order = 1)]
public class Meeting : DayAction
{
    [SerializeField] private List<AbstractEmployeeConfig> shopEmployees;
    [SerializeField] private List<AbstractRoomConfig> shopRooms;
    [SerializeField] private List<DayEvent> dayEvents;
    public override void ReleaseAction(LevelExecuter LevelExecuter, Action EndActionHandler)
    {
        LevelExecuter.TileBuilderController.ChangeGameMode(Gamemode.building);
    }
}
