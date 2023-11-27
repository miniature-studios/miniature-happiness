using Common;
using Level.GlobalTime;
using Level.Room;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
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
        public void Update(float delta_time) { }

        public Progress Progress { get; }
    }

    public struct EmployeeAmount
    {
        public int Amount;
    }

    [Serializable]
    public class TargetEmployeeAmount : ITask
    {
        [OdinSerialize]
        [AssetSelector]
        [AssetsOnly]
        [ShowInInspector]
        private IDataProvider<EmployeeAmount> employeeCount;

        [SerializeField]
        private int employeeCountTarget;

        public Progress Progress
        {
            get
            {
                int employee_amount = employeeCount.GetData().Amount;
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
        [OdinSerialize]
        [AssetSelector]
        [AssetsOnly]
        [ShowInInspector]
        private IDataProvider<MaxStress> maxStress;

        [OdinSerialize]
        public float MaxStressTarget { get; private set; }

        public Progress Progress =>
            new()
            {
                Completion = currentDuration,
                Overall = targetDuration,
                Complete = complete,
            };

        [SerializeField]
        private float targetDuration;

        private float currentDuration = .0f;
        private bool complete = false;

        public void Update(float delta_time)
        {
            if (complete)
            {
                return;
            }

            if (currentDuration > targetDuration)
            {
                complete = true;
            }
            else if (maxStress.GetData().Stress < MaxStressTarget)
            {
                currentDuration += delta_time;
            }
            else
            {
                currentDuration = 0.0f;
            }
        }
    }

    public struct RoomCountByUid
    {
        public Dictionary<string, int> CountByUid;
    }

    [Serializable]
    public class TargetRoomCount : ITask
    {
        [SerializeField]
        private float timeToEnsureCompletion = 0.5f;

        [OdinSerialize]
        [AssetSelector]
        [AssetsOnly]
        [ShowInInspector]
        private IDataProvider<RoomCountByUid> roomCount;

        [OdinSerialize]
        [AssetSelector]
        [AssetsOnly]
        [ShowInInspector]
        private CoreModel room;
        public string RoomTitle => room.Title;

        [SerializeField]
        private int targetAmount;

        public Progress Progress
        {
            get
            {
                roomCountCache = roomCount.GetData();

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
            new() { CountByUid = new Dictionary<string, int>() };

        private float currentEnsuringTime = 0.0f;
        private bool complete = false;

        public void Update(float delta_time)
        {
            if (complete)
            {
                return;
            }

            if (roomCountCache.CountByUid.ContainsKey(room.Uid))
            {
                if (roomCountCache.CountByUid[room.Uid] < targetAmount)
                {
                    currentEnsuringTime = 0.0f;
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
        [OdinSerialize]
        [AssetSelector]
        [AssetsOnly]
        [ShowInInspector]
        private IDataProvider<RoomCountByUid> roomCount;

        [OdinSerialize]
        [AssetSelector]
        [AssetsOnly]
        [ShowInInspector]
        private CoreModel room;
        public string RoomTitle => room.Title;

        [OdinSerialize]
        public int UpperBoundInclusive { get; private set; }

        [SerializeField]
        private Days timeToComplete;

        private Days completeness = Days.FromRealTimeSeconds(0.0f);

        public Progress Progress =>
            new()
            {
                Complete = completeness >= timeToComplete,
                Completion = completeness.Days_,
                Overall = timeToComplete.Days_
            };

        public void Update(float delta_time)
        {
            if (completeness > timeToComplete)
            {
                return;
            }

            Dictionary<string, int> count_by_id = roomCount.GetData().CountByUid;
            int currentCount = 0;
            if (count_by_id.ContainsKey(room.Uid))
            {
                currentCount = count_by_id[room.Uid];
            }

            if (currentCount <= UpperBoundInclusive)
            {
                completeness += Days.FromRealTimeSeconds(delta_time);
            }
            else
            {
                completeness = Days.FromRealTimeSeconds(0);
            }
        }
    }
}
