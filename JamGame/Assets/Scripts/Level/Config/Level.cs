using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;

namespace Level.Config
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class DayConfig
    {
        [OdinSerialize]
        public IEnumerable<IDayAction> DayActions { get; private set; } = new List<IDayAction>();
    }
}
