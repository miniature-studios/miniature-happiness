using Common;
using Level.Config;
using System;
using System.Collections;
using UnityEngine;

namespace Level
{
    [AddComponentMenu("Level.Executor")]
    public class Executor : MonoBehaviour
    {
        [SerializeField]
        private LevelConfig levelConfig;

        [SerializeField]
        private TileBuilder.Controller tileBuilderController;

        [SerializeField]
        private Finances.Model financesController;

        [SerializeField]
        private Shop.Controller shopController;

        [SerializeField]
        private UIController uiController;

        [SerializeField]
        private TarrifsCounter tarrifsCounter;

        [SerializeField]
        private Tarrifs.View dailyBillPanel;

        [SerializeField]
        private TemporaryData levelTemperaryData;

        [SerializeField]
        private TransitionPanel transitionPanel;

        private Action bufferAction;

        public void Execute(DayEnd day_end, Action next_action)
        {
            Check check = tarrifsCounter.GetCheck(levelConfig.Tariffs);
            Result result = financesController.TryTakeMoney(check.Sum);
            if (result.Success)
            {
                levelTemperaryData.CreateCheck(check);
                bufferAction = next_action;
                transitionPanel.SetText("Day end start.");
                uiController.PlayDayActionStart(day_end.GetType(), null);
            }
            else
            {
                // TODO lose game
            }
        }

        // Calls by button continue on daily bill panel
        public void CompleteDayEnd()
        {
            transitionPanel.SetText("Day end end.");
            uiController.PlayDayActionEnd(bufferAction);
        }

        public void Execute(DayStart day_start, Action next_action)
        {
            financesController.AddMoney(day_start.MorningMoney);
            transitionPanel.SetText("Day start start.");
            uiController.PlayDayActionStart(
                day_start.GetType(),
                () => _ = StartCoroutine(DayStartRoutine(1, next_action))
            );
        }

        private IEnumerator DayStartRoutine(float time, Action next_action)
        {
            yield return new WaitForSeconds(time);
            transitionPanel.SetText("Day start end.");
            uiController.PlayDayActionEnd(next_action);
        }

        public void Execute(Meeting meeting, Action next_action)
        {
            tileBuilderController.ChangeGameMode(TileBuilder.GameMode.Build);
            shopController.SetShopRooms(meeting.ShopRooms);
            shopController.SetShopEmployees(meeting.ShopEmployees);
            transitionPanel.SetText("Meeting start.");
            uiController.PlayDayActionStart(meeting.GetType(), null);
            bufferAction = next_action;
        }

        // Calls by button complete meeting
        public void CompleteMeeting()
        {
            transitionPanel.SetText("Meeting end.");
            uiController.PlayDayActionEnd(bufferAction);
            bufferAction = null;
        }

        public void Execute(Working working, Action next_action)
        {
            transitionPanel.SetText("Working start.");
            uiController.PlayDayActionStart(
                working.GetType(),
                () => _ = StartCoroutine(WorkingTime(working.WorkingTime, next_action))
            );
        }

        private IEnumerator WorkingTime(float time, Action endWorkingTime)
        {
            yield return new WaitForSeconds(time);
            transitionPanel.SetText("Working end.");
            uiController.PlayDayActionEnd(endWorkingTime);
        }
    }
}
