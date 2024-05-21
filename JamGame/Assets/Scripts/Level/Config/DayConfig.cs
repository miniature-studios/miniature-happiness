using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Config
{
    public enum ConfigMode
    {
        Raw,
        ConfigLink
    }

    [Serializable]
    [HideReferenceObjectPicker]
    public class DayConfig
    {
        [HideLabel]
        [SerializeField]
        [EnumToggleButtons]
        private ConfigMode configMode = ConfigMode.Raw;

        [ShowIf(nameof(configMode), ConfigMode.Raw)]
        [SerializeReference]
        private List<IDayAction> dayActions = new();

        [ShowIf(nameof(configMode), ConfigMode.ConfigLink)]
        [SerializeField]
        private DayConfigBundle bundle;

        public IEnumerable<IDayAction> DayActions => GetDayActions();

        private IEnumerable<IDayAction> GetDayActions()
        {
            return configMode switch
            {
                ConfigMode.Raw => dayActions,
                ConfigMode.ConfigLink => bundle.DayActions,
                _ => throw new ArgumentException()
            };
        }
    }
}
