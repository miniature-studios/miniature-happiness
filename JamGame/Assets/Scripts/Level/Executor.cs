﻿using System;
using System.Collections;
using System.Collections.Generic;
using AnimatorsSwitcher;
using Common;
using Level.Config;
using Level.GlobalTime;
using Location;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    public struct AllEmployeesAtHome
    {
        public bool Value;
    }

    public struct AllEmployeesAtMeeting
    {
        public bool Value;
    }

    [AddComponentMenu("Scripts/Level/Level.Executor")]
    public class Executor : MonoBehaviour
    {
        [SerializeField]
        private TileBuilder.Controller.ControllerImpl tileBuilderController;

        [SerializeField]
        private Finances.Model financesModel;

        [SerializeField]
        private Shop.Controller shopController;

        [SerializeField]
        private AnimatorsSwitcherImpl animatorSwitcher;

        [SerializeField]
        private DailyBill.Model dailyBill;

        [SerializeField]
        private TransitionPanel.Model transitionPanel;

        [SerializeField]
        private Boss.Model boss;

        [SerializeField]
        private NeedProviderManager needProviderManager;

        [SerializeField]
        private EmployeeManager employeeManager;

        [SerializeField]
        private GlobalTime.Model globalTime;

        [SerializeField]
        private NavMeshSurfaceUpdater navMeshUpdater;

        [SerializeField]
        private AllChildrenNeedModifiersApplier meetingStartNeedOverride;

        [SerializeField]
        private AllChildrenNeedModifiersApplier meetingEndNeedOverride;

        [SerializeField]
        private AllChildrenNeedModifiersApplier leaveNeedOverride;

        [SerializeField]
        private AllChildrenNeedModifiersApplier goToWorkNeedOverride;

        public UnityEvent ActionEndNotify;
        public UnityEvent DayEnds;

        private bool transitionPanelShown = false;
        private bool cutsceneMinTimeEnded = false;

        public void Execute(DayStart dayStart)
        {
            navMeshUpdater.UpdateNavMesh();
            needProviderManager.InitGameMode();

            // NOTE: It's a temporary solution while we don't have proper save/load system.
            leaveNeedOverride.Unregister();
            meetingEndNeedOverride.Unregister();
            goToWorkNeedOverride.Register();

            _ = StartCoroutine(employeeManager.TurnOnAllEmployees(2f));

            financesModel.AddMoney(dayStart.MorningMoney);
            animatorSwitcher.SetAnimatorStates(typeof(DayStart));
            _ = StartCoroutine(DayStartRoutine(dayStart.Duration));
        }

        private IEnumerator DayStartRoutine(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            ActionEndNotify?.Invoke();
        }

        public void Execute(PreMeeting preMeeting)
        {
            navMeshUpdater.UpdateNavMesh();
            needProviderManager.InitGameMode();
            meetingStartNeedOverride.Register();

            this.CreateGate(
                new List<Func<bool>>()
                {
                    () =>
                        DataProviderServiceLocator
                            .FetchDataFromSingleton<AllEmployeesAtMeeting>()
                            .Value
                },
                new List<Action>() { () => Debug.Log("PreMeeting"), ActionEndNotify.Invoke }
            );
        }

        public void Execute(Meeting meeting)
        {
            Result set_time_scale_lock_result = globalTime.SetTimeScaleLock(this, 0.0f);
            if (set_time_scale_lock_result.Failure)
            {
                Debug.LogError(
                    "Cannot change time scale before meeting: " + set_time_scale_lock_result.Error
                );
            }

            tileBuilderController.ChangeGameMode(TileBuilder.GameMode.Build);
            shopController.SetShopRooms(meeting.ShopRooms);
            shopController.SetShopEmployees(meeting.ShopEmployees);
            animatorSwitcher.SetAnimatorStates(typeof(Meeting));
            boss.ActivateNextTaskBunch();
        }

        // Called by button complete meeting.
        public void CompleteMeeting()
        {
            navMeshUpdater.UpdateNavMesh();

            meetingStartNeedOverride.Unregister();
            meetingEndNeedOverride.Register();

            needProviderManager.InitGameMode();
            tileBuilderController.ChangeGameMode(TileBuilder.GameMode.Play);

            Result remove_time_scale_lock_result = globalTime.RemoveTimeScaleLock(this);
            if (remove_time_scale_lock_result.Failure)
            {
                Debug.LogError(
                    "Cannot change time scale before meeting: "
                        + remove_time_scale_lock_result.Error
                );
            }

            ActionEndNotify?.Invoke();
        }

        public void Execute(Working working)
        {
            animatorSwitcher.SetAnimatorStates(typeof(Working));
            _ = StartCoroutine(WorkingTime(working.Duration));
        }

        private IEnumerator WorkingTime(Days duration)
        {
            yield return new WaitForSeconds(duration.RealTimeSeconds);
            ActionEndNotify?.Invoke();
        }

        public void Execute(Cutscene cutscene)
        {
            transitionPanel.PanelText = cutscene.Text;
            animatorSwitcher.SetAnimatorStates(typeof(Cutscene));

            transitionPanelShown = false;

            this.CreateGate(
                new List<Func<bool>>() { () => transitionPanelShown, () => cutsceneMinTimeEnded },
                new List<Action>() { ActionEndNotify.Invoke, () => cutsceneMinTimeEnded = false }
            );
            _ = StartCoroutine(CutsceneRoutine(cutscene.Duration));
        }

        private IEnumerator CutsceneRoutine(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            cutsceneMinTimeEnded = true;
        }

        public void Execute(PreDayEnd preDayEnd)
        {
            goToWorkNeedOverride.Unregister();
            leaveNeedOverride.Register();

            this.CreateGate(
                new List<Func<bool>>()
                {
                    () =>
                        DataProviderServiceLocator
                            .FetchDataFromSingleton<AllEmployeesAtHome>()
                            .Value
                },
                new List<Action>() { () => Debug.Log("PreDayEnd"), ActionEndNotify.Invoke }
            );
        }

        public void Execute(DayEnd dayEnd)
        {
            if (financesModel.TryTakeMoney(dailyBill.ComputeCheck().Sum).Failure)
            {
                Execute(new LoseGame());
                return;
            }
            animatorSwitcher.SetAnimatorStates(typeof(DayEnd));
        }

        // Called by button continue on daily bill panel.
        public void CompleteDayEnd()
        {
            DayEnds?.Invoke();
            ActionEndNotify?.Invoke();
        }

        // Called by TransitionPanel animator.
        public void TransitionPanelShown()
        {
            transitionPanelShown = true;
        }

        public void Execute(LoseGame loseGame)
        {
            animatorSwitcher.SetAnimatorStates(typeof(LoseGame));
        }

        public void Execute(WinGame winGame)
        {
            animatorSwitcher.SetAnimatorStates(typeof(WinGame));
        }

        public void Execute(LoadLevel loadLevel)
        {
            tileBuilderController.LoadBuildingFromConfig(loadLevel.BuildingConfig);
            ActionEndNotify?.Invoke();
        }
    }
}
