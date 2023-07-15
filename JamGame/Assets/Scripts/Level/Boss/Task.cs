using Common;
using System;
using UnityEngine;

namespace Level.Boss.Task
{
    [InterfaceEditor]
    public interface ITask
    {
        public void ValidateProviders();
        public bool IsComplete();
    }

    // TODO: Make it global? (to use in the systems that collect global data)
    public interface IDataProvider<D>
    {
        D GetData();
    }

    public struct EmployeeAmount
    {
        public int Amount;
    }

    [Serializable]
    public class TargetEmployeeAmount : ITask
    {
        [SerializeField] private MonoBehaviour employeeCountProvider;
        private IDataProvider<EmployeeAmount> employeeCount;

        [SerializeField] private int employeeCountTarget;

        public bool IsComplete()
        {
            return employeeCount.GetData().Amount >= employeeCountTarget;
        }

        public void ValidateProviders()
        {
            if (employeeCountProvider is not null and IDataProvider<EmployeeAmount> dp)
            {
                employeeCount = dp;
            }
            else
            {
                Debug.LogError("IDataProvider<EmployeeAmount> is not implemented for employeeCountProvider or employeeCountProvider is null");
            }
        }
    }
}