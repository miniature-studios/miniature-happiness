using System;
using System.Collections.Generic;
using Common;
using Sirenix.OdinInspector;
using TileBuilder;
using UnityEngine;

namespace Level.Room
{
    [Serializable]
    [HideLabel]
    [FoldoutGroup(nameof(GeneralRoomInfo))]
    public struct GeneralRoomInfo
    {
        public string Title;
        public RentCost RentCost;

        [TextArea]
        public string Description;
    }

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
    public partial class CoreModel : MonoBehaviour, IUidHandle, IUidPostprocessingHandle
    {
        private static string coreModelsLabel = "CoreModel";
        private static Dictionary<InternalUid, CoreModel> uidPrefabsMap = new();

        [SerializeField]
        private InternalUid uid;
        public InternalUid Uid => uid;

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
        private GeneralRoomInfo generalRoomInfo;
        public GeneralRoomInfo RoomInfo => generalRoomInfo;

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
            uidPrefabsMap = AddressableTools.LoadAllGameObjectAssets<CoreModel>(coreModelsLabel);
        }

        public static CoreModel InstantiateCoreModel(TileConfig config)
        {
            UpdateUidPrefabsMap();
            CoreModel core = Instantiate(uidPrefabsMap[config.Uid]);
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
