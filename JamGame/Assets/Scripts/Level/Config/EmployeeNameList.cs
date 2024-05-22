using System;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    [CreateAssetMenu(fileName = "EmployeeNameList", menuName = "Level/EmployeeNameList")]
    public class EmployeeNameList : ScriptableObject
    {
        [SerializeField]
        private List<string> firstNames = new();

        [SerializeField]
        private List<string> lastNames = new();

        public string GenerateName()
        {
            string firstName = firstNames[UnityEngine.Random.Range(0, firstNames.Count)];
            string lastName = lastNames[UnityEngine.Random.Range(0, lastNames.Count)];
            return firstName + " " + lastName;
        }
    }
}
