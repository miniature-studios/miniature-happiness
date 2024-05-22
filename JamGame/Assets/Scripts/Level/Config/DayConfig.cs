using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class DayConfig
    {
        private enum ConfigMode
        {
            Raw,
            ConfigLink
        }

        [HideLabel]
        [SerializeField]
        [EnumToggleButtons]
        private ConfigMode configMode = ConfigMode.Raw;

        [ShowIf(nameof(configMode), ConfigMode.Raw)]
        [SerializeReference]
        private List<IDayAction> dayActions = new();

        [ShowIf(nameof(configMode), ConfigMode.ConfigLink)]
        [SerializeField]
        private DayConfigPreset preset;

        public IEnumerable<IDayAction> DayActions => GetDayActions();

        private IEnumerable<IDayAction> GetDayActions()
        {
            return configMode switch
            {
                ConfigMode.Raw => dayActions,
                ConfigMode.ConfigLink => preset.DayActions,
                _ => throw new ArgumentException()
            };
        }
    }
}
