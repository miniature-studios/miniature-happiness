using Common;
using Employee;
using System;
using System.Collections.Generic;
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
        public EmployeeImpl Employee { get; }

        public EmployeeConfig(EmployeeImpl employee, string name)
        {
            Name = name;
            Employee = employee;
        }
    }

    [Serializable]
    public class FixedEmployeeConfig : IEmployeeConfig
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private EmployeeImpl employee;

        public EmployeeConfig GetEmployeeConfig()
        {
            return new EmployeeConfig(employee, name);
        }
    }

    [Serializable]
    public class EmployeeWeights
    {
        public float Weight;
        public EmployeeImpl Employee;
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
            EmployeeImpl result = employeeWeights[
                RandomTools.RandomlyChooseWithWeights(list)
            ].Employee;

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
}