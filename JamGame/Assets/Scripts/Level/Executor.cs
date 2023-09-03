﻿using Common;
using Level.Config;
using Level.GlobalTime;
using Location;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    public struct DaysLived
    {
        public Days Value;
    }

    public struct AllEmployeesAtHome
    {
        public bool Value;
    }

    public struct AllEmployeesAtMeeting
    {
        public bool Value;
    }

    [AddComponentMenu("Scripts/Level.Executor")]
    public class Executor : MonoBehaviour, IDataProvider<DaysLived>
    {
        [SerializeField]
        private TileBuilder.Controller.ControllerImpl tileBuilderController;

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
        private LoseGamePanel.Model loseGame;

        [SerializeField]
        private WinGamePanel.Model winGame;

        [SerializeField]
        private AllChildrenNeedModifiersApplier meetingStartNeedOverride;

        [SerializeField]
        private AllChildrenNeedModifiersApplier meetingEndNeedOverride;

        [SerializeField]
        private AllChildrenNeedModifiersApplier leaveNeedOverride;

        [SerializeField]
        private AllChildrenNeedModifiersApplier goToWorkNeedOverride;

        [SerializeField]
        private GameObject homeConditionProvider;
        private IDataProvider<AllEmployeesAtHome> homeCondition;

        [SerializeField]
        private GameObject meetingConditionProvider;
        private IDataProvider<AllEmployeesAtMeeting> meetingCondition;

        public UnityEvent ActionEndNotify;

        public SerializedEmployeeConfig TestEmployeeConfig;
        private bool transitionPanelShown = false;

        private void Awake()
        {
            homeCondition = homeConditionProvider.GetComponent<IDataProvider<AllEmployeesAtHome>>();
            if (homeCondition == null)
            {
                Debug.LogError(
                    "IDataProvider<AllEmployeesAtHome> not found in homeConditionProvider"
                );
            }
            meetingCondition = meetingConditionProvider.GetComponent<
                IDataProvider<AllEmployeesAtMeeting>
            >();
            if (meetingCondition == null)
            {
                Debug.LogError(
                    "IDataProvider<AllEmployeesAtMeeting> not found in meetingConditionProvider"
                );
            }
        }

        public void Execute(DayStart dayStart)
        {
            location.InitGameMode();

            for (int i = 0; i < 1; i++)
            {
                location.AddEmployee(TestEmployeeConfig.ToEmployeeConfig().GetEmployeeConfig());
            }

            // NOTE: It's a temportary solution while we don't have proper save/load system.
            leaveNeedOverride.Unregister();
            goToWorkNeedOverride.Register();

            financesModel.AddMoney(dayStart.MorningMoney);
            animatorSwitcher.SetAnimatorStates(typeof(DayStart));
            _ = StartCoroutine(DayStartRoutine(dayStart.Duration));
        }

        private IEnumerator DayStartRoutine(float time)
        {
            yield return new WaitForSeconds(time);
            ActionEndNotify?.Invoke();
        }

        public void Execute(PreMeeting preMeeting)
        {
            meetingStartNeedOverride.Register();

            this.CreateGate(
                new List<Func<bool>>() { () => meetingCondition.GetData().Value },
                new List<Action>() { () => Debug.Log("PreMeeting"), ActionEndNotify.Invoke }
            );
        }

        public void Execute(Meeting meeting)
        {
            tileBuilderController.ChangeGameMode(TileBuilder.Controller.GameMode.Build);
            shopController.SetShopRooms(meeting.ShopRooms);
            shopController.SetShopEmployees(meeting.ShopEmployees);
            animatorSwitcher.SetAnimatorStates(typeof(Meeting));
            boss.ActivateNextTaskBunch();
        }

        // Called by button complete meeting.
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

            transitionPanelShown = false;

            this.CreateGate(
                new List<Func<bool>>() { () => transitionPanelShown },
                new List<Action>() { ActionEndNotify.Invoke }
            );
        }

        public void Execute(PreDayEnd preDayEnd)
        {
            leaveNeedOverride.Register();

            this.CreateGate(
                new List<Func<bool>>() { () => homeCondition.GetData().Value },
                new List<Action>() { () => Debug.Log("PreDayEnd"), ActionEndNotify.Invoke }
            );
        }

        public void Execute(DayEnd dayEnd)
        {
            tariffsCounter.UpdateCheck();
            if (financesModel.TryTakeMoney(tariffsCounter.Check.Sum).Failure)
            {
                Execute(new LoseGame());
                return;
            }
            animatorSwitcher.SetAnimatorStates(typeof(DayEnd));
        }

        // Called by button continue on daily bill panel.
        public void CompleteDayEnd()
        {
            ActionEndNotify?.Invoke();
        }

        // Called by TransitionPanel animator.
        public void TransitionPanelShown()
        {
            transitionPanelShown = true;
        }

        public void Execute(LoseGame loseGame)
        {
            this.loseGame.PrepareToShow();
            animatorSwitcher.SetAnimatorStates(typeof(LoseGame));
        }

        public void Execute(WinGame winGame)
        {
            this.winGame.PrepareToShow();
            animatorSwitcher.SetAnimatorStates(typeof(WinGame));
        }

        public DaysLived GetData()
        {
            //TODO: Implement
            throw new NotImplementedException();
        }
    }
}
