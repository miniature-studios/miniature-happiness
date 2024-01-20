using System;
using System.Collections.Generic;
using Level.Config.DayAction;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class DayConfig
    {
        [SerializeReference]
        private List<IDayAction> dayActions = new();
        public IEnumerable<IDayAction> DayActions => dayActions;
    }
}
