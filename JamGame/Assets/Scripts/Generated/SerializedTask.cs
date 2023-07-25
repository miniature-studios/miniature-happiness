using System;
using UnityEngine;

namespace Level.Boss.Task 
{
    [Serializable]
    public class SerializedTask
    {
        [SerializeField] 
        private string selectedType;

        [SerializeField]
        private TargetEmployeeAmount targetEmployeeAmount;

        [SerializeField]
        private MaxStressBound maxStressBound;

        public ITask ToTask()
        {
            return selectedType switch
            {
                "TargetEmployeeAmount" => targetEmployeeAmount,
                "MaxStressBound" => maxStressBound,
                _ => throw new NotImplementedException(),
            };
        }
    }
}