using System;
using System.Collections.Generic;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    [CreateAssetMenu(fileName = "DayConfigBundle", menuName = "Level/DayConfigBundle")]
    internal class DayConfigBundle : ScriptableObject
    {
        [SerializeReference]
        private List<IDayAction> dayActions = new();
        public IEnumerable<IDayAction> DayActions => dayActions;
    }
}
