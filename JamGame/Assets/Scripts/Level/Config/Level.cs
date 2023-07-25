using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    public class DayConfig
    {
        [SerializeField]
        private List<SerializedDayAction> rawDayActions;
        private List<IDayAction> dayActions = null;
        public ReadOnlyCollection<IDayAction> DayActions
        {
            get
            {
                if (dayActions == null)
                {
                    dayActions = new();
                    dayActions.AddRange(rawDayActions.Select(x => x.ToDayAction()));
                }
                return dayActions.AsReadOnly();
            }
        }
    }

    [Serializable]
    public class Tariffs
    {
        [SerializeField]
        private int waterCost;

        [SerializeField]
        private int electricityCost;

        [SerializeField]
        private int rentCost;
        public int WaterCost => waterCost;
        public int ElectricityCost => electricityCost;
        public int RentCost => rentCost;
    }
}
