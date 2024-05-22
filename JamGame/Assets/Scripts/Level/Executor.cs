using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnimatorsSwitcher;
using Common;
using Employee.Needs;
using Level.Config;
using Level.GlobalTime;
using Location;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    public struct GameLoseCause
    {
        public LoseGame.Cause Cause;
    }

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
        private TileBuilder.Controller tileBuilderController;

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
        private Location.EmployeeManager.Model employeeManager;

        [SerializeField]
        private GlobalTime.Model globalTime;

        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        [SerializeField]
        private Overlay.Controller offOverlayController;

        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        [SerializeField]
        private LoseGamePanel.Model loseGamePanelModel;

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

        public event Action ActionEndNotify;

        private bool transitionPanelShown = false;
        private bool cutsceneMinTimeEnded = false;
        private bool isGameFinished = false;
        private bool isPreMeetingEnd = false;

        [SerializeField]
        private UnityEvent dayEnded;

        private void Awake()
        {
            tileBuilderController.BuiltValidatedOffice += CompleteMeeting;
        }

        private void Update()
        {
            IEnumerable<GameLoseCause> causes =
                DataProviderServiceLocator.FetchDataFromMultipleSources<GameLoseCause>();
            if (causes.Count() > 0 && !isGameFinished)
            {
                Execute(new LoseGame(causes.First().Cause));
            }
        }

        public void Execute(DayStart dayStart)
        {
            navMeshUpdater.UpdateNavMesh();
            needProviderManager.InitGameMode();

            // NOTE: It's a temporary solution while we don't have proper save/load system.
            leaveNeedOverride.Unregister();
            meetingEndNeedOverride.Unregister();
            goToWorkNeedOverride.Register();

            _ = StartCoroutine(employeeManager.TurnOnAllEmployees(dayStart.EmployeeEnableDelay));

            financesModel.AddMoney(dayStart.MorningMoney);
            animatorSwitcher.SetAnimatorStates(typeof(DayStart));
            _ = StartCoroutine(DayStartRoutine(dayStart.Duration));
        }

        private IEnumerator DayStartRoutine(RealTimeSeconds time)
        {
            yield return new WaitForSecondsRealtime(time.Value);
            ActionEndNotify?.Invoke();
        }

        public void Execute(PreMeeting preMeeting)
        {
            navMeshUpdater.UpdateNavMesh();
            needProviderManager.InitGameMode();

            meetingEndNeedOverride.Unregister();
            meetingStartNeedOverride.Register();

            isPreMeetingEnd = false;
            _ = StartCoroutine(WaitRoutine(preMeeting.MinWaitingTime));

            if (globalTime.IsLocked)
            {
                Result timeScaleLockResult = globalTime.RemoveTimeScaleLock(this);
                timeScaleLockResult.LogErrorIfFailure("Cannot remove timeScaleLock in Pre meeting");
            }

            this.CreateGate(
                new List<Func<bool>>()
                {
                    () =>
                        DataProviderServiceLocator
                            .FetchDataFromSingleton<AllEmployeesAtMeeting>()
                            .Value,
                    () => isPreMeetingEnd
                },
                new List<Action>()
                {
                    () =>
                    {
                        Result timeLockResult = globalTime.SetTimeScaleLock(this, 0f);
                        timeLockResult.LogErrorIfFailure(
                            "Cannot set time scale lock in preMeeting End."
                        );
                    },
                    () => offOverlayController.ForceSetState(true),
                    ActionEndNotify.Invoke
                }
            );
        }

        private IEnumerator WaitRoutine(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            isPreMeetingEnd = true;
        }

        public void Execute(Meeting meeting)
        {
            tileBuilderController.ChangeGameMode(TileBuilder.GameMode.Build);
            shopController.SetShopRooms(meeting.ShopRooms);
            shopController.SetShopEmployees(meeting.ShopEmployees);
            animatorSwitcher.SetAnimatorStates(typeof(Meeting));
            boss.ActivateNextTaskBunch();
        }

        public void CompleteMeeting()
        {
            navMeshUpdater.UpdateNavMesh();

            meetingStartNeedOverride.Unregister();
            meetingEndNeedOverride.Register();

            needProviderManager.InitGameMode();
            tileBuilderController.ChangeGameMode(TileBuilder.GameMode.Play);

            IEnumerable<NeedProvider> meeting_need_providers =
                needProviderManager.FindAllNeedProvidersOfType(NeedType.Meeting);
            foreach (NeedProvider need_provider in meeting_need_providers)
            {
                need_provider.ForceReleaseEmployeeIfAny();
            }

            ActionEndNotify?.Invoke();
        }

        public void Execute(Working working)
        {
            animatorSwitcher.SetAnimatorStates(typeof(Working));
            if (globalTime.IsLocked)
            {
                Result timeScaleLockResult = globalTime.RemoveTimeScaleLock(this);
                timeScaleLockResult.LogErrorIfFailure(
                    "Cannot remove timeScaleLock in Start Working"
                );
            }
            _ = StartCoroutine(WorkingTime(working.Duration));
        }

        private IEnumerator WorkingTime(Days duration)
        {
            yield return new WaitForSeconds(duration.RealTimeSeconds.Value);
            Result timeLockResult = globalTime.SetTimeScaleLock(this, 0f);
            timeLockResult.LogErrorIfFailure("Cannot set time scale lock in End Working.");
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

        private IEnumerator CutsceneRoutine(RealTimeSeconds time)
        {
            yield return new WaitForSecondsRealtime(time.Value);
            cutsceneMinTimeEnded = true;
        }

        public void Execute(PreDayEnd preDayEnd)
        {
            goToWorkNeedOverride.Unregister();
            leaveNeedOverride.Register();

            if (globalTime.IsLocked)
            {
                Result timeScaleLockResult = globalTime.RemoveTimeScaleLock(this);
                timeScaleLockResult.LogErrorIfFailure(
                    "Cannot remove timeScaleLock in Start Working"
                );
            }

            this.CreateGate(
                new List<Func<bool>>()
                {
                    () =>
                        DataProviderServiceLocator
                            .FetchDataFromSingleton<AllEmployeesAtHome>()
                            .Value
                },
                new List<Action>()
                {
                    () =>
                    {
                        Result timeLockResult = globalTime.SetTimeScaleLock(this, 0f);
                        timeLockResult.LogErrorIfFailure(
                            "Cannot set time scale lock in End Working."
                        );
                    },
                    () => offOverlayController.ForceSetState(true),
                    ActionEndNotify.Invoke
                }
            );
        }

        public void Execute(DayEnd dayEnd)
        {
            if (financesModel.TryTakeMoney(dailyBill.ComputeCheck().Sum).Failure)
            {
                LoseGame loseGame = new(LoseGame.Cause.NegativeMoney);
                Execute(loseGame);
                return;
            }

            animatorSwitcher.SetAnimatorStates(typeof(DayEnd));
        }

        // Called by button continue on daily bill panel.
        public void CompleteDayEnd()
        {
            dayEnded.Invoke();

            if (boss.AllTasksAreComplete())
            {
                Execute(new WinGame());
                return;
            }

            ActionEndNotify?.Invoke();
        }

        // Called by TransitionPanel animator.
        public void TransitionPanelShown()
        {
            transitionPanelShown = true;
        }

        public void Execute(LoseGame loseGame)
        {
            isGameFinished = true;
            loseGamePanelModel.SetCause(loseGame.LoseCause);
            animatorSwitcher.SetAnimatorStates(typeof(LoseGame));
        }

        public void Execute(WinGame winGame)
        {
            isGameFinished = true;
            animatorSwitcher.SetAnimatorStates(typeof(WinGame));
        }

        public void Execute(LoadLevel loadLevel)
        {
            isGameFinished = false;
            tileBuilderController.LoadBuildingFromConfig(loadLevel.BuildingConfig);
            animatorSwitcher.SetAnimatorStates(typeof(LoadLevel));
            Result timeLockResult = globalTime.SetTimeScaleLock(this, 0f);
            timeLockResult.LogErrorIfFailure("Cannot set time scale lock in Load level.");
            ActionEndNotify?.Invoke();
        }
    }
}
