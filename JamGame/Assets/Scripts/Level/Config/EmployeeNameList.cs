using System;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    [CreateAssetMenu(
        fileName = "EmployeeNameGenerator",
        menuName = "Level/EmployeeNameGenerator",
        order = 0
    )]
    public class EmployeeNameList : ScriptableObject
    {
        // TODO: Wrap fields as readonly.

        [SerializeField]
        public List<string> FirstNames;

        [SerializeField]
        public List<string> LastNames;
    }
}
