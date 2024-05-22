using System;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    [CreateAssetMenu(fileName = "DayConfigPreset", menuName = "Level/DayConfigPreset")]
    internal class DayConfigPreset : ScriptableObject
    {
        [SerializeReference]
        private List<IDayAction> dayActions = new();
        public IEnumerable<IDayAction> DayActions => dayActions;
    }
}
