#if UNITY_EDITOR
using Common;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Level.Room
{
    public partial class CoreModel
    {
        public void CatchModels()
        {
            shopModel = GetComponent<Shop.Room.Model>();
            inventoryModel = GetComponent<Inventory.Room.Model>();
            tileUnionModel = GetComponent<TileUnion.Model>();
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(CoreModel))]
    public class CoreModelInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            CoreModel coreModel = serializedObject.targetObject as CoreModel;

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Catch Models"))
            {
                coreModel.CatchModels();
                EditorUtility.SetDirty(coreModel);
            }
            EditorGUILayout.EndHorizontal();

            List<Shop.Room.View> shopRooms = new();
            List<Inventory.Room.View> inventoryViews = new();
            List<TileUnion.TileUnionImpl> tileUnions = new();

            foreach (
                AssetWithLocation<Shop.Room.View> shopView in AddressableTools<Shop.Room.View>.LoadAllFromLabel(
                    "ShopView"
                )
            )
            {
                if (shopView.Asset.HashCode == coreModel.Uid)
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
                if (invView.Asset.HashCode == coreModel.Uid)
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
                if (tileUnion.Asset.HashCode == coreModel.Uid)
                {
                    tileUnions.Add(tileUnion.Asset);
                }
            }

            _ = EditorGUILayout.BeginHorizontal();
            GUILayout.Label("All Shop room views dependencies: ", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            if (shopRooms.Count == 0)
            {
                _ = EditorGUILayout.BeginHorizontal();
                GUILayout.Label("None dependencies.");
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                foreach (Shop.Room.View item in shopRooms)
                {
                    _ = EditorGUILayout.BeginHorizontal();
                    _ = EditorGUILayout.ObjectField(
                        "Shop.Room.View: ",
                        item,
                        typeof(Shop.Room.View),
                        false
                    );
                    EditorGUILayout.EndHorizontal();
                }
            }

            _ = EditorGUILayout.BeginHorizontal();
            GUILayout.Label("All Inventory views dependencies: ", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            if (inventoryViews.Count == 0)
            {
                _ = EditorGUILayout.BeginHorizontal();
                GUILayout.Label("None dependencies.");
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                foreach (Inventory.Room.View item in inventoryViews)
                {
                    _ = EditorGUILayout.BeginHorizontal();
                    _ = EditorGUILayout.ObjectField(
                        "Inventory.Room.View: ",
                        item,
                        typeof(Inventory.Room.View),
                        false
                    );
                    EditorGUILayout.EndHorizontal();
                }
            }

            _ = EditorGUILayout.BeginHorizontal();
            GUILayout.Label("All TileUnions dependencies: ", EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
            if (tileUnions.Count == 0)
            {
                _ = EditorGUILayout.BeginHorizontal();
                GUILayout.Label("None dependencies.");
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                foreach (TileUnion.TileUnionImpl item in tileUnions)
                {
                    _ = EditorGUILayout.BeginHorizontal();
                    _ = EditorGUILayout.ObjectField(
                        "Inventory.Room.View: ",
                        item,
                        typeof(TileUnion.TileUnionImpl),
                        false
                    );
                    EditorGUILayout.EndHorizontal();
                }
            }

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
