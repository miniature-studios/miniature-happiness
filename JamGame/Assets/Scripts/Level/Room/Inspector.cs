#if UNITY_EDITOR
using Common;
using Sirenix.OdinInspector;
using System.Collections.Generic;
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

        [OnInspectorGUI]
        private void FindViews()
        {
            shopRooms.Clear();
            inventoryViews.Clear();
            tileUnions.Clear();
            foreach (
                AssetWithLocation<Shop.Room.View> shopView in AddressableTools<Shop.Room.View>.LoadAllFromLabel(
                    "ShopView"
                )
            )
            {
                if (shopView.Asset.Uid == Uid)
                {
                    shopRooms.Add(shopView.Asset);
                }
            }

            foreach (
                AssetWithLocation<Inventory.Room.View> invView in AddressableTools<Inventory.Room.View>.LoadAllFromLabel(
                    "InventoryView"
                )
            )
            {
                if (invView.Asset.Uid == Uid)
                {
                    inventoryViews.Add(invView.Asset);
                }
            }

            foreach (
                AssetWithLocation<TileUnion.TileUnionImpl> tileUnion in AddressableTools<TileUnion.TileUnionImpl>.LoadAllFromLabel(
                    "TileUnion"
                )
            )
            {
                if (tileUnion.Asset.Uid == Uid)
                {
                    tileUnions.Add(tileUnion.Asset);
                }
            }
        }

        [ReadOnly]
        [SerializeField]
        [Title("All Shop room views dependencies: ")]
        private List<Shop.Room.View> shopRooms = new();

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
