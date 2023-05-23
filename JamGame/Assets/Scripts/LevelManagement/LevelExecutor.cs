using Common;
using System;
using System.Collections;
using UnityEngine;

public class LevelExecutor : MonoBehaviour
{
    [SerializeField] private TileBuilderController tileBuilderController;
    [SerializeField] private Finances financesController;
    [SerializeField] private ShopController shopController;
    [SerializeField] private UIController uIController;
    private void Awake()
    {
        tileBuilderController.BuildedValidatedOffice += CompleteMeeting;
    }
    public void ExecuteDayAction(IDayAction day_action)
    {
        switch (day_action)
        {
            case DayStart: Execute(day_action as DayStart); break;
            case DayEnd: Execute(day_action as DayEnd); break;
            case Meeting: Execute(day_action as Meeting); break;
            case Working: Execute(day_action as Working); break;
            default: throw new ArgumentException();
        }
    }
    private void Execute(DayStart day_start)
    {
        uIController.SetUIState(UIController.UIState.AllHidden);
        day_start.ActionEnd();
    }

    private void Execute(DayEnd day_end)
    {
        uIController.SetUIState(UIController.UIState.AllHidden);
        day_end.ActionEnd();
    }

    private Meeting currentMeeting = null;
    private void Execute(Meeting meeting)
    {
        tileBuilderController.ChangeGameMode(Gamemode.Building);
        shopController.SetShopRooms(meeting.ShopRooms);
        shopController.SetShopEmployees(meeting.ShopEmployees);
        uIController.SetUIState(UIController.UIState.ForMeeting);
        currentMeeting = meeting;
    }

    private void CompleteMeeting()
    {
        currentMeeting.ActionEnd();
        currentMeeting = null;
    }

    private void Execute(Working working)
    {
        uIController.SetUIState(UIController.UIState.ForWorking);
        _ = StartCoroutine(WorkingTime(working.WorkingTime, working.ActionEnd));
    }

    private IEnumerator WorkingTime(float time, Action endWorkingTime)
    {
        yield return new WaitForSeconds(time);
        endWorkingTime();
    }
}

