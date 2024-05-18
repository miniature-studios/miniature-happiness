using System;
using System.Collections.Generic;
using Employee;
using Employee.Personality;
using Pickle;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Config
{
    public interface IEmployeeConfig
    {
        public EmployeeConfig GetEmployeeConfig();
    }

    public class EmployeeConfig
    {
        public string Name;
        public int HireCost;
        public string Profession;
        public List<Quirk> Quirks;
    }

    [Serializable]
    public class FixedEmployeeConfig : IEmployeeConfig
    {
        [SerializeField]
        [FoldoutGroup("@" + nameof(Label))]
        private string name;

        [SerializeField]
        private int hireCost;

        [SerializeField]
        private string profession;

        [SerializeField]
        private List<Quirk> quirks;

        private string Label => $"Employee - {name}";

        public EmployeeConfig GetEmployeeConfig()
        {
            return new EmployeeConfig()
            {
                Name = name,
                HireCost = hireCost,
                Profession = profession,
                Quirks = quirks
            };
        }
    }

    [Serializable]
    [HideReferenceObjectPicker]
    public class EmployeeWeights
    {
        public float Weight;

        [AssetsOnly]
        [Pickle(typeof(EmployeeImpl), LookupType = ObjectProviderType.Assets)]
        public GameObject Prototype;
    }

    [Serializable]
    public class RandomEmployeeConfig : IEmployeeConfig
    {
        [SerializeField]
        [Pickle(typeof(EmployeeWeightList), LookupType = ObjectProviderType.Assets)]
        [FoldoutGroup("Employee - Random")]
        private EmployeeWeightList weightList;

        [SerializeField]
        [Pickle(typeof(EmployeeNameList), LookupType = ObjectProviderType.Assets)]
        [FoldoutGroup("Employee - Random")]
        private EmployeeNameList nameList;

        public EmployeeConfig GetEmployeeConfig()
        {
            // TODO: #48
            return new EmployeeConfig()
            {
                Name = "Fook",
                HireCost = 100,
                Profession = "ProGear",
                Quirks = null
            };
        }
    }
}
