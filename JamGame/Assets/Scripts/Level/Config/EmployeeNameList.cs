using System;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    [CreateAssetMenu(fileName = "EmployeeNameList", menuName = "Level/EmployeeNameList", order = 0)]
    public class EmployeeNameList : ScriptableObject
    {
        [SerializeField]
        private List<string> firstNames = new();
        public IEnumerable<string> FirstNames => firstNames;

        [SerializeField]
        private List<string> lastNames = new();
        public IEnumerable<string> LastNames => lastNames;
    }
}
