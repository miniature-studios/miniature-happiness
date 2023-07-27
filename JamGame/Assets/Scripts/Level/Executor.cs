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
        // FIXME: dublication with sheduler, whant tarrifs
        [SerializeField]
        private LevelConfig levelConfig;

        [SerializeField]
        private TileBuilder.Controller tileBuilderController;

        [SerializeField]
        private Finances.Model financesModel;

        [SerializeField]
        private Shop.Controller shopController;

        [SerializeField]
        private AnimatorsSwitcher animatorSwitcher;

        [SerializeField]
        private TarrifsCounter tarrifsCounter;

        [SerializeField]
        private TemporaryData levelTemperaryData;

        [SerializeField]
        private TransitionPanel transitionPanel;

        [SerializeField]
        private Boss.Model boss;

        private Action bufferAction;

        public void Execute(DayEnd day_end, Action next_action)
        {
            Check check = tarrifsCounter.GetCheck(levelConfig.Tariffs);
            Result result = financesModel.TryTakeMoney(check.Sum);
            if (result.Success)
            {
                levelTemperaryData.CreateCheck(check);
                bufferAction = next_action;
                animatorSwitcher.SetAnimatorStates(typeof(DayEnd));
            }
            else
            {
                // TODO lose game
            }
        }

        // Calls by button continue on daily bill panel
        public void CompleteDayEnd()
        {
            bufferAction();
        }

        public void Execute(DayStart day_start, Action next_action)
        {
            financesModel.AddMoney(day_start.MorningMoney);
            animatorSwitcher.SetAnimatorStates(typeof(DayStart));
            _ = StartCoroutine(DayStartRoutine(3, next_action));
        }

        private IEnumerator DayStartRoutine(float time, Action next_action)
        {
            yield return new WaitForSeconds(time);
            next_action();
        }

        public void Execute(Meeting meeting, Action next_action)
        {
            tileBuilderController.ChangeGameMode(TileBuilder.GameMode.Build);
            shopController.SetShopRooms(meeting.ShopRooms);
            shopController.SetShopEmployees(meeting.ShopEmployees);
            animatorSwitcher.SetAnimatorStates(typeof(Meeting));
            boss.ActivateNextTaskBunch();
            bufferAction = next_action;
        }

        // Calls by button complete meeting
        public void CompleteMeeting()
        {
            bufferAction();
            bufferAction = null;
        }

        public void Execute(Working working, Action next_action)
        {
            animatorSwitcher.SetAnimatorStates(typeof(Working));
            _ = StartCoroutine(WorkingTime(working.WorkingTime, next_action));
        }

        private IEnumerator WorkingTime(float time, Action endWorkingTime)
        {
            yield return new WaitForSeconds(time);
            endWorkingTime();
        }

        public void Execute(Cutscene cutscene, Action next_action)
        {
            transitionPanel.SetText(cutscene.Text);
            animatorSwitcher.SetAnimatorStates(typeof(Cutscene));
            _ = StartCoroutine(CutsceneRoutine(cutscene.Duration, next_action));
        }

        public IEnumerator CutsceneRoutine(float time, Action nextAction)
        {
            yield return new WaitForSeconds(time);
            nextAction();
        }
    }
}
