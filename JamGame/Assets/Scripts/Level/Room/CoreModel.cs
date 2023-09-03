using Common;
using System;
using System.Collections.Generic;
using TileBuilder;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

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
        private static string coreModelsLabel = "CoreModel";
        private static Dictionary<string, IResourceLocation> uidPrefabsMap = new();

        [SerializeField]
        [InspectorReadOnly]
        // TODO: Wrap it in newtype.
        private string uid;
        public string Uid => uid;

#if UNITY_EDITOR
        public void SetHashCode(string uid)
        {
            this.uid = uid;
        }
#endif

        [SerializeField]
        private string title;
        public string Title => title;

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

        private static void UpdateUidPrefabsMap()
        {
            if (uidPrefabsMap.Count == 0)
            {
                foreach (
                    AssetWithLocation<CoreModel> core in AddressableTools<CoreModel>.LoadAllFromLabel(
                        coreModelsLabel
                    )
                )
                {
                    uidPrefabsMap.Add(core.Asset.Uid, core.Location);
                }
            }
        }

        public static CoreModel InstantiateCoreModel(TileConfig config)
        {
            UpdateUidPrefabsMap();
            CoreModel core = Instantiate(
                AddressableTools<CoreModel>.LoadAsset(uidPrefabsMap[config.HashCode])
            );
            core.TileUnionModel.PlacingProperties.SetPositionAndRotation(
                config.Position,
                config.Rotation
            );
            return core;
        }

        public static CoreModel InstantiateCoreModel(string hashCode)
        {
            return InstantiateCoreModel(new TileConfig(hashCode, Vector2Int.zero, 0));
        }
    }
}
