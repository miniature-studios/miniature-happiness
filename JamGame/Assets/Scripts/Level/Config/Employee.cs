using Common;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level.Config
{
    [HideReferenceObjectPicker]
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
        [FoldoutGroup("@Label")]
        private string name;

        [SerializeField]
        [AssetsOnly]
        [AssetSelector]
        [FoldoutGroup("@Label")]
        private GameObject prototype;

        private string Label => $"Employee - {name}";

        public EmployeeConfig GetEmployeeConfig()
        {
            return new EmployeeConfig(prototype, name);
        }
    }

    [Serializable]
    [HideReferenceObjectPicker]
    public class EmployeeWeights
    {
        public float Weight;

        [AssetsOnly]
        [AssetSelector]
        public GameObject Prototype;
    }

    [Serializable]
    public class RandomEmployeeConfig : IEmployeeConfig
    {
        [SerializeField]
        [AssetSelector]
        [FoldoutGroup("Employee - Random")]
        private EmployeeWeightList weightList;

        [SerializeField]
        [AssetSelector]
        [FoldoutGroup("Employee - Random")]
        private EmployeeNameList nameList;

        public EmployeeConfig GetEmployeeConfig()
        {
            List<float> list = weightList.EmployeeWeights.Select(x => x.Weight).ToList();
            GameObject result = weightList.EmployeeWeights.ToList()[
                RandomTools.RandomlyChooseWithWeights(list)
            ].Prototype;

            string first_name = nameList.FirstNames.OrderBy(x => UnityEngine.Random.value).First();
            string last_name = nameList.LastNames.OrderBy(x => UnityEngine.Random.value).First();
            string full_name = $"{first_name} {last_name}";

            return new EmployeeConfig(result, full_name);
        }
    }
}
