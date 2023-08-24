using Common;
using System;
using UnityEngine;

namespace Level.Room
{
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

    [RequireComponent(typeof(Shop.Room.Model))]
    [RequireComponent(typeof(TileUnion.Model))]
    [RequireComponent(typeof(Inventory.Room.Model))]
    [AddComponentMenu("Scripts/Level.Room.CoreModel")]
    public partial class CoreModel : MonoBehaviour
    {
        [SerializeField]
        [InspectorReadOnly]
        private string hashCode;
        public string HashCode => hashCode;

        [SerializeField]
        [InspectorReadOnly]
        private Shop.Room.Model shopModel;
        public Shop.Room.Model ShopModel => shopModel;

        [SerializeField]
        [InspectorReadOnly]
        private Inventory.Room.Model inventoryModel;
        public Inventory.Room.Model InventoryModel => inventoryModel;

        [SerializeField]
        [InspectorReadOnly]
        private TileUnion.Model tileUnionModel;
        public TileUnion.Model TileUnionModel => tileUnionModel;

        [SerializeField]
        private TariffProperties tariffProperties;
        public TariffProperties TariffProperties => tariffProperties;
    }
}
