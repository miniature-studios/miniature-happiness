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
            inventoryViews = GetFromLabel<Inventory.Room.View>("InventoryView");
            tileUnions = GetFromLabel<TileUnion.TileUnionImpl>("TileUnion");
        }

        private List<T> GetFromLabel<T>(string label)
            where T : MonoBehaviour, IUidHandle
        {
            List<T> result = new();
            Dictionary<InternalUid, T> dictionary = AddressableTools.LoadAllGameObjectAssets<T>(
                label
            );
            foreach (KeyValuePair<InternalUid, T> asset in dictionary)
            {
                if (asset.Value.Uid == Uid)
                {
                    result.Add(asset.Value);
                }
            }
            return result;
        }

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
