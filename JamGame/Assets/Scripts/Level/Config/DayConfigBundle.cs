using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    [CreateAssetMenu(fileName = "DayConfigBundle", menuName = "Level/DayConfigBundle")]
    internal class DayConfigBundle : ScriptableObject
    {
        [SerializeField]
        [InlineProperty]
        [HideLabel]
        private DayConfig dayConfig = new();
        public IEnumerable<IDayAction> DayActions => dayConfig.DayActions;
    }
}
