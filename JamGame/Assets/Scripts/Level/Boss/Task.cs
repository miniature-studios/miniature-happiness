using Common;
using System;
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

        public Progress GetProgress();
    }

    public struct EmployeeAmount
    {
        public int Amount;
    }

    [Serializable]
    public class TargetEmployeeAmount : ITask
    {
        [SerializeField] private GameObject employeeCountProvider;
        private IDataProvider<EmployeeAmount> employeeCount;

        [SerializeField] private int employeeCountTarget;

        public void ValidateProviders()
        {
            employeeCount = employeeCountProvider.GetComponent<IDataProvider<EmployeeAmount>>();
            if (employeeCount == null)
            {
                Debug.LogError("IDataProvider<EmployeeAmount>  not found in employeeCountProvider");
            }
        }

        public Progress GetProgress()
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

    public struct MaxStress
    {
        public float Stress;
    }

    [Serializable]
    public class MaxStressBound : ITask
    {
        [SerializeField] private GameObject maxStressProvider;
        private IDataProvider<MaxStress> maxStress;

        [SerializeField] private float maxStressTarget;
        public float MaxStressTarget => maxStressTarget;

        [SerializeField] private float targetDuration;

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

        public Progress GetProgress()
        {
            return new Progress()
            {
                Completion = currentDuration,
                Overall = targetDuration,
                Complete = complete,
            };
        }
    }
}
