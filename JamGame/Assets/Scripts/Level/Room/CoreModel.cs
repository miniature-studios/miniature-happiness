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
    public partial class CoreModel : MonoBehaviour, IUidHandle
    {
        private static string coreModelsLabel = "CoreModel";
        private static Dictionary<InternalUid, IResourceLocation> uidPrefabsMap = new();

        [SerializeField]
        [InlineProperty]
        private InternalUid uid;
        public InternalUid Uid => uid;

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
#if UNITY_EDITOR
            uidPrefabsMap.Clear();
            ForceUpdateUidPrefabsMap();
#else
            if (uidPrefabsMap.Count == 0)
            {
                ForceUpdateUidPrefabsMap();
            }
#endif
        }

        private static void ForceUpdateUidPrefabsMap()
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

        public static CoreModel InstantiateCoreModel(TileConfig config)
        {
            UpdateUidPrefabsMap();
            CoreModel core = Instantiate(
                AddressableTools<CoreModel>.LoadAsset(uidPrefabsMap[config.Uid])
            );
            core.TileUnionModel.PlacingProperties.SetPositionAndRotation(
                config.Position,
                config.Rotation
            );
            return core;
        }

        public static CoreModel InstantiateCoreModel(InternalUid uid)
        {
            return InstantiateCoreModel(new TileConfig(uid, Vector2Int.zero, 0));
        }
    }
}
