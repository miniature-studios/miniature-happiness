using Common;
using System;
using UnityEngine;

namespace Level.Room
{
    [Serializable]
    public struct Cost
    {
        [SerializeField]
        private int cost;
        public int Value => cost;
    }

    [Serializable]
    public struct TariffProperties
    {
        [SerializeField]
        private int waterConsumption;
        public int WaterConsumption => waterConsumption;

        [SerializeField]
        private int electricityConsumption;
        public int ElectricityConsumption => electricityConsumption;
    }

    [AddComponentMenu("Scripts/Level.Room.CoreModel")]
    public partial class CoreModel : MonoBehaviour
    {
        [SerializeField]
        private UniqueId uniqueId;
        public UniqueId UniqueId => uniqueId;

        [SerializeField]
        private TileUnion.TileUnionImpl tileUnionPrefab;
        public TileUnion.TileUnionImpl TileUnionPrefab => tileUnionPrefab;

        [SerializeField]
        private Cost cost;
        public Cost Cost => cost;

        [SerializeField]
        private TariffProperties tariffProperties;
        public TariffProperties TariffProperties => tariffProperties;
    }
}
