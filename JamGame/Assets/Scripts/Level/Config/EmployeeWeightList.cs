using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Config
{
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
}
