using System;
using System.Collections.Generic;
using UnityEngine;

namespace Employee.Personality
{
    [Serializable]
    [CreateAssetMenu(fileName = "NameList", menuName = "Employee/NameList")]
    public class NameList : ScriptableObject
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
