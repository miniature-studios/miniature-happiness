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
        private RoomsBuilt roomsBuilt;

        [SerializeField]
        private MaxStressBound maxStressBound;

        public ITask ToTask()
        {
            return selectedType switch
            {
                "TargetEmployeeAmount" => targetEmployeeAmount,
                "RoomsBuilt" => roomsBuilt,
                "MaxStressBound" => maxStressBound,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
