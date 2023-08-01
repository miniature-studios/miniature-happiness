using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    public class DayConfig
    {
        [SerializeField]
        private List<SerializedDayAction> rawDayActions;

        public ImmutableList<IDayAction> DayActions =>
            rawDayActions.Select(x => x.ToDayAction()).ToImmutableList();
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
