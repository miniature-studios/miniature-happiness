using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Employee;
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
        [FoldoutGroup("@" + nameof(Label))]
        private string name;

        [SerializeField]
        private int hireCost;

        [SerializeField]
        private string profession;

        [SerializeField]
        private string quirk;

        [AssetsOnly]
        [Pickle(typeof(EmployeeImpl), LookupType = ObjectProviderType.Assets)]
        [FoldoutGroup("@" + nameof(Label))]
        private GameObject prototype;

        private string Label => $"Employee - {name}";

        public EmployeeConfig GetEmployeeConfig()
        {
            return new EmployeeConfig(name, hireCost, profession, quirk);
        }
    }

    [Serializable]
    [HideReferenceObjectPicker]
    public class EmployeeWeights
    {
        public float Weight;

        //[AssetsOnly]
        //[Pickle(typeof(EmployeeImpl), LookupType = ObjectProviderType.Assets)]
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

        [SerializeField]
        private int minCost;

        [SerializeField]
        private int maxCost;

        public EmployeeConfig GetEmployeeConfig()
        {
            List<float> list = weightList.EmployeeWeights.Select(x => x.Weight).ToList();
            GameObject result = weightList
                .EmployeeWeights.ToList()[RandomTools.RandomlyChooseWithWeights(list)]
                .Prototype;

            string first_name = nameList.FirstNames.OrderBy(x => UnityEngine.Random.value).First();
            string last_name = nameList.LastNames.OrderBy(x => UnityEngine.Random.value).First();
            string full_name = $"{first_name} {last_name}";

            // TODO: refactor
            //return new EmployeeConfig(result, full_name);
            return new EmployeeConfig(
                full_name,
                UnityEngine.Random.Range(minCost, maxCost),
                "Proger",
                "No quirk"
            );
        }
    }
}
