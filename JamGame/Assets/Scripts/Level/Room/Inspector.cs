#if UNITY_EDITOR
using System.Collections.Generic;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Room
{
    public partial class CoreModel
    {
        [Button]
        private void CatchModels()
        {
            shopModel = GetComponent<Shop.Room.Model>();
            inventoryModel = GetComponent<Inventory.Room.Model>();
            tileUnionModel = GetComponent<TileUnion.Model>();
        }

        [Button]
        [OnInspectorGUI]
        private void FindViews()
        {
            shopRoomPlanks = GetFromLabel<Shop.Room.Plank>("ShopRoomPlanks");
            shopRoomCards = GetFromLabel<Shop.Room.Card>("ShopRoomCards");
            inventoryViews = GetFromLabel<Inventory.Room.View>("InventoryView");
            tileUnions = GetFromLabel<TileUnion.TileUnionImpl>("TileUnion");
        }

        private List<T> GetFromLabel<T>(string label)
            where T : MonoBehaviour, IUidHandle
        {
            List<T> result = new();
            IEnumerable<AssetWithLocation<T>> list = AddressableTools<T>.LoadAllFromLabel(label);
            foreach (AssetWithLocation<T> asset in list)
            {
                if (asset.Asset.Uid == Uid)
                {
                    result.Add(asset.Asset);
                }
            }
            return result;
        }

        [ReadOnly]
        [SerializeField]
        [Title("All Shop room planks dependencies: ")]
        private List<Shop.Room.Plank> shopRoomPlanks = new();

        [ReadOnly]
        [SerializeField]
        [Title("All Shop room cards dependencies: ")]
        private List<Shop.Room.Card> shopRoomCards = new();

        [ReadOnly]
        [SerializeField]
        [Title("All Inventory views dependencies: ")]
        private List<Inventory.Room.View> inventoryViews = new();

        [ReadOnly]
        [SerializeField]
        [Title("All TileUnions dependencies: ")]
        private List<TileUnion.TileUnionImpl> tileUnions = new();
    }
}
#endif
