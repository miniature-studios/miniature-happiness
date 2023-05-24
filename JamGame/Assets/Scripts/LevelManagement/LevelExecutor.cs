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

    #region Day Start
    private void Execute(DayStart day_start)
    {
        financesController.AddMoney(day_start.MorningMoney);
        uIController.SetUIState(UIController.UIState.AllHidden);
        uIController.HideTransitionPanel("Day starts early...", 1,
            () => StartCoroutine(DayStartAnimation(2, day_start.ActionEnd)));
    }

    private IEnumerator DayStartAnimation(float time, Action animation_end)
    {
        yield return new WaitForSeconds(time);
        animation_end();
    }
    #endregion

    #region Meeting
    private Meeting currentMeeting = null;
    private void Execute(Meeting meeting)
    {
        uIController.ShowTransitionPanel("And now,starts meeting...", 1, () =>
        {

            tileBuilderController.ChangeGameMode(Gamemode.Building);
            shopController.SetShopRooms(meeting.ShopRooms);
            shopController.SetShopEmployees(meeting.ShopEmployees);
            uIController.SetUIState(UIController.UIState.ForMeeting);
            uIController.HideTransitionPanel("And now,starts meeting...", 1, null);
            currentMeeting = meeting;
        });
    }

    private void CompleteMeeting()
    {
        currentMeeting.ActionEnd();
        currentMeeting = null;
    }
    #endregion

    #region Working
    private void Execute(Working working)
    {
        uIController.ShowTransitionPanel("Working Time!!!", 1, () =>
        {
            uIController.SetUIState(UIController.UIState.ForWorking);
            _ = StartCoroutine(WorkingTime(working.WorkingTime, working.ActionEnd));
            uIController.HideTransitionPanel("Working Time!!!", 1, null);
        });

    }

    private IEnumerator WorkingTime(float time, Action endWorkingTime)
    {
        yield return new WaitForSeconds(time);
        endWorkingTime();
    }
    #endregion

    #region Day End
    private IEnumerator DayEndAnimation(float time, Action animation_end)
    {
        yield return new WaitForSeconds(time);
        animation_end();
    }

    private void Execute(DayEnd day_end)
    {
        uIController.SetUIState(UIController.UIState.AllHidden);
        _ = StartCoroutine(DayEndAnimation(2,
            () => uIController.ShowTransitionPanel("And day ends...", 1, day_end.ActionEnd)
            ));
    }
    #endregion
}

