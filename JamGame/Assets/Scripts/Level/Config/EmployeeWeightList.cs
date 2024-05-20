using System;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    [CreateAssetMenu(fileName = "EmployeeWeightList", menuName = "Level/EmployeeWeightList")]
    public class EmployeeWeightList : ScriptableObject
    {
        [SerializeField]
        private List<EmployeeWeights> employeeWeights = new();
        public IEnumerable<EmployeeWeights> EmployeeWeights => employeeWeights;
    }
}
