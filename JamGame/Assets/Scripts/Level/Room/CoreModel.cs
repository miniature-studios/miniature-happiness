using Common;
using TileBuilder;
using UnityEngine;

namespace Level.Room
{
    [RequireComponent(typeof(RoomInformation))]
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
        private RoomInformation roomInformation;
        public RoomInformation RoomInformation => roomInformation;

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

        public void ConfigurateFromConfig(TileConfig config)
        {
            tileUnionModel.PlacingProperties.SetPositionAndRotation(
                config.Position,
                config.Rotation
            );
        }
    }
}
