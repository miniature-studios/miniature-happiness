using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Employee.Needs;
using Employee.Personality;
using Level.Finances;
using Level.GlobalTime;
using Level.Room;
using Pickle;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Boss.Task
{
    public struct Progress
    {
        public float Completion;
        public float Overall;
        public bool Complete;
    }

    [HideReferenceObjectPicker]
    public interface ITask
    {
        public void Init() { }

        public void Update(RealTimeSeconds delta_time) { }

        public Progress Progress { get; }
    }

    public struct EmployeeAmount
    {
        public int Amount;
    }

    [Serializable]
    public class TargetEmployeeAmount : ITask
    {
        [SerializeField]
        [FoldoutGroup("Target Employee Amount")]
        private int employeeCountTarget;

        public Progress Progress
        {
            get
            {
                int employee_amount = DataProviderServiceLocator
                    .FetchDataFromSingleton<EmployeeAmount>()
                    .Amount;

                return new Progress
                {
                    Completion = employee_amount,
                    Overall = employeeCountTarget,
                    Complete = employee_amount >= employeeCountTarget
                };
            }
        }
    }

    public struct MaxStress
    {
        public float Stress;
    }

    [Serializable]
    public class MaxStressBound : ITask
    {
        [SerializeField]
        [FoldoutGroup("Max Stress Bound")]
        private float maxStressTarget;
        public float MaxStressTarget => maxStressTarget;

        public Progress Progress =>
            new()
            {
                Completion = currentDuration.Value,
                Overall = targetDuration.Value,
                Complete = complete,
            };

        [SerializeField]
        [FoldoutGroup("Max Stress Bound")]
        private Days targetDuration;

        private Days currentDuration = Days.Zero;
        private bool complete = false;

        public void Update(RealTimeSeconds delta_time)
        {
            if (complete)
            {
                return;
            }

            if (currentDuration > targetDuration)
            {
                complete = true;
                return;
            }

            float max_stress = DataProviderServiceLocator
                .FetchDataFromSingleton<MaxStress>()
                .Stress;
            if (max_stress < MaxStressTarget)
            {
                currentDuration += new Days(delta_time);
            }
            else
            {
                currentDuration = Days.Zero;
            }
        }
    }

    public struct RoomCountByUid
    {
        public Dictionary<InternalUid, int> CountByUid;
    }

    [Serializable]
    public class TargetRoomCount : ITask
    {
        [SerializeField]
        [MinValue(0)]
        [FoldoutGroup("Target Room Count")]
        private RealTimeSeconds timeToEnsureCompletion = new(0.5f);

        [AssetsOnly]
        [SerializeField]
        [Pickle(typeof(CoreModel), LookupType = ObjectProviderType.Assets)]
        [FoldoutGroup("Target Room Count")]
        private CoreModel room;
        public string RoomTitle => room.RoomInfo.Title;

        [SerializeField]
        [FoldoutGroup("Target Room Count")]
        private int targetAmount;

        public Progress Progress
        {
            get
            {
                roomCountCache =
                    DataProviderServiceLocator.FetchDataFromSingleton<RoomCountByUid>();

                int completion = 0;
                if (roomCountCache.CountByUid.ContainsKey(room.Uid))
                {
                    completion = roomCountCache.CountByUid[room.Uid];
                }

                return new()
                {
                    Complete = complete,
                    Completion = completion,
                    Overall = targetAmount
                };
            }
        }

        private RoomCountByUid roomCountCache =
            new() { CountByUid = new Dictionary<InternalUid, int>() };

        private RealTimeSeconds currentEnsuringTime = RealTimeSeconds.Zero;
        private bool complete = false;

        public void Update(RealTimeSeconds delta_time)
        {
            if (complete)
            {
                return;
            }

            if (roomCountCache.CountByUid.ContainsKey(room.Uid))
            {
                if (roomCountCache.CountByUid[room.Uid] < targetAmount)
                {
                    currentEnsuringTime = RealTimeSeconds.Zero;
                }
                else
                {
                    currentEnsuringTime += delta_time;
                }
            }

            if (currentEnsuringTime > timeToEnsureCompletion)
            {
                complete = true;
            }
        }
    }

    [Serializable]
    public class RoomCountUpperBound : ITask
    {
        [AssetsOnly]
        [SerializeField]
        [Pickle(typeof(CoreModel), LookupType = ObjectProviderType.Assets)]
        [FoldoutGroup("Room Count Upper Bound")]
        private CoreModel room;
        public string RoomTitle => room.RoomInfo.Title;

        [SerializeField]
        [FoldoutGroup("Room Count Upper Bound")]
        private int upperIntBoundInclusive;
        public int UpperBoundInclusive => upperIntBoundInclusive;

        [SerializeField]
        [FoldoutGroup("Room Count Upper Bound")]
        private Days timeToComplete;

        private Days completeness = Days.Zero;

        public Progress Progress =>
            new()
            {
                Complete = completeness >= timeToComplete,
                Completion = completeness.Value,
                Overall = timeToComplete.Value
            };

        public void Update(RealTimeSeconds delta_time)
        {
            if (completeness > timeToComplete)
            {
                return;
            }

            Dictionary<InternalUid, int> count_by_id = DataProviderServiceLocator
                .FetchDataFromSingleton<RoomCountByUid>()
                .CountByUid;
            int currentCount = 0;
            if (count_by_id.ContainsKey(room.Uid))
            {
                currentCount = count_by_id[room.Uid];
            }

            if (currentCount <= UpperBoundInclusive)
            {
                completeness += new Days(delta_time);
            }
            else
            {
                completeness = Days.Zero;
            }
        }
    }

    [Serializable]
    public class MinBalance : ITask
    {
        [SerializeField]
        [FoldoutGroup("Min Balance")]
        private float minBalanceTarget;
        public float MinBalanceTarget => minBalanceTarget;

        public Progress Progress =>
            new()
            {
                Completion = currentDuration.Value,
                Overall = targetDuration.Value,
                Complete = complete,
            };

        [SerializeField]
        [FoldoutGroup("Min Balance")]
        private Days targetDuration;

        private Days currentDuration = Days.Zero;
        private bool complete = false;

        public void Update(RealTimeSeconds delta_time)
        {
            if (complete)
            {
                return;
            }

            if (currentDuration > targetDuration)
            {
                complete = true;
                return;
            }

            float money = DataProviderServiceLocator.FetchDataFromSingleton<Money>().Value;
            if (money >= minBalanceTarget)
            {
                currentDuration += new Days(delta_time);
            }
            else
            {
                currentDuration = Days.Zero;
            }
        }
    }

    public struct EmployeeQuirks
    {
        public IEnumerable<Quirk> Quirks;
    }

    [Serializable]
    public class MinEmployeesWithQuirk : ITask
    {
        [SerializeField]
        [FoldoutGroup("Min Employee Count With Quirk")]
        private int employeeCountTarget;
        public int EmployeeCountTarget => employeeCountTarget;

        public Progress Progress =>
            new()
            {
                Completion = currentCount,
                Overall = employeeCountTarget,
                Complete = complete,
            };

        [SerializeField]
        [AssetsOnly]
        [Pickle(typeof(Quirk), LookupType = ObjectProviderType.Assets)]
        [FoldoutGroup("Min Employee Count With Quirk")]
        private Quirk targetQuirk;

        private int currentCount = 0;
        private bool complete = false;

        public void Update(RealTimeSeconds delta_time)
        {
            if (complete)
            {
                return;
            }

            currentCount = 0;
            IEnumerable<EmployeeQuirks> employee_quirks =
                DataProviderServiceLocator.FetchDataFromMultipleSources<EmployeeQuirks>();
            foreach (EmployeeQuirks quirks in employee_quirks)
            {
                if (quirks.Quirks.Contains(targetQuirk))
                {
                    currentCount++;
                }
            }

            if (currentCount >= employeeCountTarget)
            {
                complete = true;
            }
        }
    }

    public struct WaitingLineLength
    {
        public int Value;
    }

    [Serializable]
    public class MaxWaitingLineLength : ITask
    {
        [SerializeField]
        [FoldoutGroup("Max Waiting Line Length")]
        private int lengthTarget;
        public int LengthTarget => lengthTarget;

        public Progress Progress =>
            new()
            {
                Completion = currentDuration.Value,
                Overall = targetDuration.Value,
                Complete = complete,
            };

        [SerializeField]
        [FoldoutGroup("Max Waiting Line Length")]
        private Days targetDuration;

        private Days currentDuration = Days.Zero;
        private bool complete = false;

        public void Update(RealTimeSeconds delta_time)
        {
            if (complete)
            {
                return;
            }

            int currentMaxLength = DataProviderServiceLocator
                .FetchDataFromMultipleSources<WaitingLineLength>()
                .Select(data => data.Value)
                .DefaultIfEmpty()
                .Max();

            if (currentMaxLength <= lengthTarget)
            {
                currentDuration += new Days(delta_time);
            }
            else
            {
                currentDuration = Days.Zero;
            }

            if (currentDuration >= targetDuration)
            {
                complete = true;
            }
        }
    }

    [Serializable]
    public class DontSatisfyNeed : ITask
    {
        [SerializeField]
        [Required]
        [FoldoutGroup("Dont Satisfy Need")]
        private Location.EmployeeManager.Model employeeManager;

        [SerializeField]
        [FoldoutGroup("Dont Satisfy Need")]
        private NeedType targetNeed;
        public NeedType TargetNeed => targetNeed;

        public Progress Progress =>
            new()
            {
                Completion = currentDuration.Value,
                Overall = targetDuration.Value,
                Complete = complete,
            };

        [SerializeField]
        [FoldoutGroup("Dont Satisfy Need")]
        private Days targetDuration;

        private Days currentDuration = Days.Zero;
        private bool complete = false;

        public void Init()
        {
            employeeManager.EmployeeNeedSatisfied += (_, need) =>
            {
                if (need == targetNeed && !complete)
                {
                    currentDuration = Days.Zero;
                }
            };
        }

        public void Update(RealTimeSeconds delta_time)
        {
            if (complete)
            {
                return;
            }

            currentDuration += new Days(delta_time);
            if (currentDuration > targetDuration)
            {
                complete = true;
                return;
            }
        }
    }

    [Serializable]
    public class MinEarnPerWorkingSession : ITask
    {
        [SerializeField]
        [RequiredIn(PrefabKind.PrefabInstance)]
        [FoldoutGroup("Min Earn Per Working Session")]
        private Executor executor;

        [SerializeField]
        [FoldoutGroup("Min Earn Per Working Session")]
        private float earnTarget;

        public Progress Progress =>
            new()
            {
                Completion = currentEarned - workingTimeStartEarned,
                Overall = earnTarget,
                Complete = complete,
            };

        private bool complete = false;
        private float workingTimeStartEarned;
        private float currentEarned = 0.0f;

        public void Init()
        {
            workingTimeStartEarned = DataProviderServiceLocator
                .FetchDataFromSingleton<MoneyEarned>()
                .Value;
            executor.ActionEndNotify += () => workingTimeStartEarned = currentEarned;
        }

        public void Update(RealTimeSeconds delta_time)
        {
            if (complete)
            {
                return;
            }

            currentEarned = DataProviderServiceLocator.FetchDataFromSingleton<MoneyEarned>().Value;

            if (currentEarned - workingTimeStartEarned >= earnTarget)
            {
                complete = true;
            }
        }
    }
}
