using Common;
using Level.GlobalTime;
using Level.Room;
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

    [InterfaceEditor]
    public interface ITask
    {
        public void ValidateProviders();

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
        [SerializeField]
        private GameObject employeeCountProvider;
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

        public void ValidateProviders()
        {
            employeeCount = employeeCountProvider.GetComponent<IDataProvider<EmployeeAmount>>();
            if (employeeCount == null)
            {
                Debug.LogError("IDataProvider<EmployeeAmount>  not found in employeeCountProvider");
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
        private GameObject maxStressProvider;
        private IDataProvider<MaxStress> maxStress;

        [SerializeField]
        private float maxStressTarget;
        public float MaxStressTarget => maxStressTarget;

        public Progress Progress => new()
        {
            Completion = currentDuration,
            Overall = targetDuration,
            Complete = complete,
        };

        [SerializeField]
        private float targetDuration;

        private float currentDuration = .0f;
        private bool complete = false;

        public void ValidateProviders()
        {
            maxStress = maxStressProvider.GetComponent<IDataProvider<MaxStress>>();
            if (maxStress == null)
            {
                Debug.LogError("IDataProvider<MaxStress> not found in maxStressProvider");
            }
        }

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
            else if (maxStress.GetData().Stress < maxStressTarget)
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

        [SerializeField]
        private GameObject roomCountProvider;
        private IDataProvider<RoomCountByUid> roomCount;

        [SerializeField]
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

        private RoomCountByUid roomCountCache = new() { CountByUid = new Dictionary<string, int>() };

        private float currentEnsuringTime = 0.0f;
        private bool complete = false;

        public void ValidateProviders()
        {
            roomCount = roomCountProvider.GetComponent<IDataProvider<RoomCountByUid>>();
            if (roomCount == null)
            {
                Debug.LogError("IDataProvider<RoomCountByUid> not found in roomCountProvider");
            }
        }

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
        [SerializeField]
        private GameObject roomCountProvider;
        private IDataProvider<RoomCountByUid> roomCount;

        [SerializeField]
        private CoreModel room;
        public string RoomTitle => room.Title;

        [SerializeField]
        private int upperBoundInclusive;
        public int UpperBoundInclusive => upperBoundInclusive;

        [SerializeField]
        private Days timeToComplete;

        private Days completeness = Days.FromRealTimeSeconds(0.0f);

        public Progress Progress => new()
        {
            Complete = completeness >= timeToComplete,
            Completion = completeness.Days_,
            Overall = timeToComplete.Days_
        };

        public void ValidateProviders()
        {
            roomCount = roomCountProvider.GetComponent<IDataProvider<RoomCountByUid>>();
            if (roomCount == null)
            {
                Debug.LogError("IDataProvider<RoomCountByUid> not found in roomCountProvider");
            }
        }

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

            if (currentCount <= upperBoundInclusive)
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
