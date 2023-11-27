using Common;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        [Discardable]
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
        private EmployeeWeightList employeeWeightList = new();

        [SerializeField]
        [AssetSelector]
        [FoldoutGroup("Employee - Random")]
        private EmployeeNameList nameList;

        public EmployeeConfig GetEmployeeConfig()
        {
            List<float> list = employeeWeightList.EmployeeWeights.Select(x => x.Weight).ToList();
            GameObject result = employeeWeightList.EmployeeWeights.ToList()[
                RandomTools.RandomlyChooseWithWeights(list)
            ].Prototype;

            string first_name = nameList.FirstNames.OrderBy(x => UnityEngine.Random.value).First();
            string last_name = nameList.LastNames.OrderBy(x => UnityEngine.Random.value).First();
            string full_name = $"{first_name} {last_name}";

            return new EmployeeConfig(result, full_name);
        }
    }

    [Serializable]
    [CreateAssetMenu(
        fileName = "EmployeeWeightList",
        menuName = "Level/EmployeeWeightList",
        order = 0
    )]
    public class EmployeeWeightList : SerializedScriptableObject
    {
        [OdinSerialize]
        public IEnumerable<EmployeeWeights> EmployeeWeights { get; private set; } =
            new List<EmployeeWeights>();
    }

    [Serializable]
    [CreateAssetMenu(fileName = "EmployeeNameList", menuName = "Level/EmployeeNameList", order = 0)]
    public class EmployeeNameList : SerializedScriptableObject
    {
        [OdinSerialize]
        public IEnumerable<string> FirstNames { get; private set; } = new List<string>();

        [OdinSerialize]
        public IEnumerable<string> LastNames { get; private set; } = new List<string>();
    }
}
