using Common;
using System;

public partial class LevelExecutor
{
    public void Execute(Meeting meeting, Action next_action)
    {
        tileBuilderController.ChangeGameMode(Gamemode.Build);
        shopController.SetShopRooms(meeting.ShopRooms);
        shopController.SetShopEmployees(meeting.ShopEmployees);
        transitionPanel.SetText("Meeting start.");
        uIController.PlayDayActionStart(meeting.GetType(), null);
        bufferAction = next_action;
    }

    // Calls by button complete meeting
    public void CompleteMeeting()
    {
        transitionPanel.SetText("Meeting end.");
        uIController.PlayDayActionEnd(bufferAction);
        bufferAction = null;
    }
}

