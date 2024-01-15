using System;
using System.Collections.Generic;
using Common;
using Sirenix.OdinInspector;
using TileBuilder;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Level.Room
{
    [Serializable]
    [InlineProperty]
    public struct RentCost
    {
        [SerializeField]
        private int cost;
        public int Value => cost;
    }

    [RequireComponent(typeof(Shop.Room.Model))]
    [RequireComponent(typeof(TileUnion.Model))]
    [RequireComponent(typeof(Inventory.Room.Model))]
    [AddComponentMenu("Scripts/Level/Room/Level.Room.CoreModel")]
    public partial class CoreModel : MonoBehaviour
    {
        private static string coreModelsLabel = "CoreModel";
        private static Dictionary<string, IResourceLocation> uidPrefabsMap = new();

        [ReadOnly]
        [SerializeField]
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

        [ReadOnly]
        [SerializeField]
        private Shop.Room.Model shopModel;
        public Shop.Room.Model ShopModel => shopModel;

        [ReadOnly]
        [SerializeField]
        private Inventory.Room.Model inventoryModel;
        public Inventory.Room.Model InventoryModel => inventoryModel;

        [ReadOnly]
        [SerializeField]
        private TileUnion.Model tileUnionModel;
        public TileUnion.Model TileUnionModel => tileUnionModel;

        [SerializeField]
        private RentCost rentCost;
        public RentCost RentCost => rentCost;

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
            Result<CoreModel> result = AddressableTools<CoreModel>.LoadAsset(
                uidPrefabsMap[config.HashCode]
            );
            CoreModel core = Instantiate(result.Data);
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
