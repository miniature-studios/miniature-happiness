using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace Level.Config
{
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
