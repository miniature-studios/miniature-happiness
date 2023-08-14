using Common;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine;

namespace Level.Config
{
    [InterfaceEditor]
    public interface IEmployeeConfig
    {
        public EmployeeConfig GetEmployeeConfig();
    }

    public class EmployeeConfig
    {
        public string Name { get; }
        public GameObject Prototype { get; }

        public EmployeeConfig(GameObject prototype, string name)
        {
            Name = name;
            Prototype = prototype;
        }
    }

    [Serializable]
    public class FixedEmployeeConfig : IEmployeeConfig
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private GameObject prototype;

        public EmployeeConfig GetEmployeeConfig()
        {
            return new EmployeeConfig(prototype, name);
        }
    }

    [Serializable]
    public class EmployeeWeights
    {
        public float Weight;
        public GameObject Prototype;
    }

    [Serializable]
    public class RandomEmployeeConfig : IEmployeeConfig
    {
        [SerializeField]
        private List<EmployeeWeights> employeeWeights;

        [SerializeField]
        private EmployeeNameList nameList;

        public EmployeeConfig GetEmployeeConfig()
        {
            List<float> list = employeeWeights.Select(x => x.Weight).ToList();
            GameObject result = employeeWeights[
                RandomTools.RandomlyChooseWithWeights(list)
            ].Prototype;

            string first_name = nameList.FirstNames[
                UnityEngine.Random.Range(0, nameList.FirstNames.Count)
            ];
            string last_name = nameList.LastNames[
                UnityEngine.Random.Range(0, nameList.LastNames.Count)
            ];
            string full_name = $"{first_name} {last_name}";

            return new EmployeeConfig(result, full_name);
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "EmployeeNameList", menuName = "Level/EmployeeNameList", order = 0)]
    public class EmployeeNameList : ScriptableObject
    {
        [SerializeField]
        private List<string> firstNames;
        public ImmutableList<string> FirstNames => firstNames.ToImmutableList();

        [SerializeField]
        private List<string> lastNames;
        public ImmutableList<string> LastNames => lastNames.ToImmutableList();
    }
}
