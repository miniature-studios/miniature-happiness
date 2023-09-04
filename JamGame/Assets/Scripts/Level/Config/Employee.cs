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
        public int HireCost { get; }
        public string Profession { get; }
        public string Quirk { get; }

        public EmployeeConfig(string name, int hireCost, string profession, string quirk)
        {
            Name = name;
            HireCost = hireCost;
            Profession = profession;
            Quirk = quirk;
        }
    }

    [Serializable]
    public class FixedEmployeeConfig : IEmployeeConfig
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private int hireCost;

        [SerializeField]
        private string profession;

        [SerializeField]
        private string quirk;

        public EmployeeConfig GetEmployeeConfig()
        {
            return new EmployeeConfig(name, hireCost, profession, quirk);
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

            // TODO: refactor
            //return new EmployeeConfig(result, full_name);
            return new EmployeeConfig("Fuko", 100, "Proger", "No quirk");
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
