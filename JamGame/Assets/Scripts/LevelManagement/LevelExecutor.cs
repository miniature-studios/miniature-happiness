using Common;
using System;
using System.Collections;
using UnityEngine;

public class LevelExecutor : MonoBehaviour
{
    [SerializeField] private LevelProportiesConfig levelProportiesConfig;

    [SerializeField] private TileBuilderController tileBuilderController;
    [SerializeField] private Finances financesController;
    [SerializeField] private ShopController shopController;
    [SerializeField] private UIController uIController;
    [SerializeField] private TarrifsCounter tarrifsCounter;
    [SerializeField] private DailyBillPanel dailyBillPanel;

    private void Awake()
    {
        tileBuilderController.BuildedValidatedOffice += CompleteMeeting;
        dailyBillPanel.continueButtonPress += CompleteDayEnd;
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
            () => StartCoroutine(DayStartRoutine(1, day_start.ActionEnd)));
    }

    private IEnumerator DayStartRoutine(float time, Action animation_end)
    {
        yield return new WaitForSeconds(time);
        uIController.ShowTransitionPanel("And ended early...", 1, animation_end);
    }
    #endregion

    #region Meeting
    private Meeting currentMeeting = null;
    private void Execute(Meeting meeting)
    {
        tileBuilderController.ChangeGameMode(Gamemode.Building);
        shopController.SetShopRooms(meeting.ShopRooms);
        shopController.SetShopEmployees(meeting.ShopEmployees);
        uIController.SetUIState(UIController.UIState.ForMeeting);
        uIController.HideTransitionPanel("And now, starts meeting...", 1, null);
        currentMeeting = meeting;
    }

    private void CompleteMeeting()
    {
        uIController.ShowTransitionPanel("Meeting ends...", 1, currentMeeting.ActionEnd);
        currentMeeting = null;
    }
    #endregion

    #region Working
    private void Execute(Working working)
    {
        uIController.SetUIState(UIController.UIState.ForWorking);
        uIController.HideTransitionPanel("Working Time!!!", 1,
            () => StartCoroutine(WorkingTime(working.WorkingTime, working.ActionEnd)));
    }

    private IEnumerator WorkingTime(float time, Action endWorkingTime)
    {
        yield return new WaitForSeconds(time);
        uIController.ShowTransitionPanel("Working time ends.", 1, endWorkingTime);
    }
    #endregion

    #region Day End
    private DayEnd currentDayEnd = null;
    private void Execute(DayEnd day_end)
    {
        uIController.SetUIState(UIController.UIState.AllHidden);
        Check check = tarrifsCounter.GetCheck(levelProportiesConfig.Tariffs);
        financesController.TakeMoney(check.Summ);
        uIController.ShowDailyBillPanel(check);
        uIController.HideTransitionPanel("And day ends....", 1, null);
        currentDayEnd = day_end;
    }
    private void CompleteDayEnd()
    {
        uIController.HideDailyBillPanel();
        uIController.ShowTransitionPanel("And day ended.", 1, currentDayEnd.ActionEnd);
        currentMeeting = null;
    }
    #endregion
}

