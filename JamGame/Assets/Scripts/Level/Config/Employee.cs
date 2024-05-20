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
        private string name;
        private int hireCost;
        private string profession;
        private List<Quirk> quirks;

        public string Name => name;
        public int HireCost => hireCost;
        public string Profession => profession;
        public List<Quirk> Quirks => quirks;

        public EmployeeConfig(string name, int hireCost, string profession, List<Quirk> quirks)
        {
            this.name = name;
            this.hireCost = hireCost;
            this.profession = profession;
            this.quirks = quirks;
        }
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
            return new EmployeeConfig(name, hireCost, profession, quirks);
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
            return new EmployeeConfig("Fook", 100, "ProGear", new List<Quirk>());
        }
    }
}
