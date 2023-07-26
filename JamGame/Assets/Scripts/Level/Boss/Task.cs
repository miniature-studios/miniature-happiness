using Common;
using System;
using UnityEngine;

namespace Level.Boss.Task
{
    [InterfaceEditor]
    public interface ITask
    {
        public void ValidateProviders();

        public void Update(float delta_time) { }

        public bool IsComplete();
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

        public bool IsComplete()
        {
            return employeeCount.GetData().Amount >= employeeCountTarget;
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
        [SerializeField] private GameObject maxStressProvider;
        private IDataProvider<MaxStress> maxStress;

        [SerializeField] private float maxStressTarget;
        [SerializeField] private float targetDuration;

        private float currentDuration = .0f;
        private bool complete = false;

        public bool IsComplete()
        {
            return complete;
        }

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
}