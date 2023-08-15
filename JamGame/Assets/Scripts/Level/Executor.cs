using Common;
using Level.Config;
using Location;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    [AddComponentMenu("Scripts/Level.Executor")]
    public class Executor : MonoBehaviour
    {
        [SerializeField]
        private TileBuilder.Controller tileBuilderController;

        [SerializeField]
        private Finances.Model financesModel;

        [SerializeField]
        private Shop.Controller shopController;

        [SerializeField]
        private AnimatorsSwitcher.AnimatorsSwitcher animatorSwitcher;

        [SerializeField]
        private TariffsCounter tariffsCounter;

        [SerializeField]
        private TransitionPanel.Model transitionPanel;

        [SerializeField]
        private Boss.Model boss;

        [SerializeField]
        private LocationImpl location;

        [SerializeField]
        private AllChildrenNeedModifiersApplier meetingStartNeedOverride;

        [SerializeField]
        private AllChildrenNeedModifiersApplier meetingEndNeedOverride;

        [SerializeField]
        private AllChildrenNeedModifiersApplier leaveNeedOverride;

        public UnityEvent ActionEndNotify;

        private ConditionsWaiter conditionsWaiter;

        public SerializedEmployeeConfig TestEmployeeConfig;

        private void Update()
        {
            conditionsWaiter?.CheckConditions();
        }

        public void Execute(DayStart day_start)
        {
            for (int i = 0; i < 1; i++)
            {
                location.AddEmployee(TestEmployeeConfig.ToEmployeeConfig().GetEmployeeConfig());
            }

            financesModel.AddMoney(day_start.MorningMoney);
            animatorSwitcher.SetAnimatorStates(typeof(DayStart));
            _ = StartCoroutine(DayStartRoutine(day_start.Duration));
        }

        private IEnumerator DayStartRoutine(float time)
        {
            yield return new WaitForSeconds(time);
            ActionEndNotify?.Invoke();
        }

        public void Execute(PreMeeting preMeeting)
        {
            if (location is IDataProvider<LocationImpl.AllEmployeesAtMeeting> dataProvider)
            {
                conditionsWaiter = new(
                    new List<Func<bool>>() { () => dataProvider.GetData().Value },
                    new List<Action>() { () => Debug.Log("PreMeeting"), ActionEndNotify.Invoke }
                );
            }
        }

        public void Execute(Meeting meeting)
        {
            meetingStartNeedOverride.Register();

            tileBuilderController.ChangeGameMode(TileBuilder.GameMode.Build);
            shopController.SetShopRooms(meeting.ShopRooms);
            shopController.SetShopEmployees(meeting.ShopEmployees);
            animatorSwitcher.SetAnimatorStates(typeof(Meeting));
            boss.ActivateNextTaskBunch();
        }

        // Calls by button complete meeting
        public void CompleteMeeting()
        {
            meetingStartNeedOverride.Unregister();
            meetingEndNeedOverride.Register();

            ActionEndNotify?.Invoke();
        }

        public void Execute(Working working)
        {
            animatorSwitcher.SetAnimatorStates(typeof(Working));
            _ = StartCoroutine(WorkingTime(working.Duration.RealTimeSeconds));
        }

        private IEnumerator WorkingTime(float time)
        {
            yield return new WaitForSeconds(time);
            ActionEndNotify?.Invoke();
        }

        public void Execute(Cutscene cutscene)
        {
            transitionPanel.PanelText = cutscene.Text;
            animatorSwitcher.SetAnimatorStates(typeof(Cutscene));
            _ = StartCoroutine(CutsceneRoutine(cutscene.Duration));
        }

        public IEnumerator CutsceneRoutine(float time)
        {
            yield return new WaitForSeconds(time);
            ActionEndNotify?.Invoke();
        }

        public void Execute(PreDayEnd preDayEnd)
        {
            if (location is IDataProvider<LocationImpl.AllEmployeesAtHome> dataProvider)
            {
                conditionsWaiter = new(
                    new List<Func<bool>>() { () => dataProvider.GetData().Value },
                    new List<Action>() { () => Debug.Log("PreDayEnd"), ActionEndNotify.Invoke }
                );
            }
        }

        public void Execute(DayEnd day_end)
        {
            leaveNeedOverride.Register();

            tariffsCounter.UpdateCheck();
            if (financesModel.TryTakeMoney(tariffsCounter.Check.Sum).Failure)
            {
                // TODO lose game
            }
            animatorSwitcher.SetAnimatorStates(typeof(DayEnd));
        }

        // Calls by button continue on daily bill panel
        public void CompleteDayEnd()
        {
            ActionEndNotify?.Invoke();
        }
    }
}
