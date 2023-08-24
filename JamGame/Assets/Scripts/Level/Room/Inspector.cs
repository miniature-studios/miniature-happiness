#if UNITY_EDITOR
using Common;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Level.Room
{
    public partial class CoreModel
    {
        public void SetHashCode()
        {
            hashCode = Convert.ToString(Guid.NewGuid());
        }

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
                AssetWithLocation<Shop.Room.View> pair in AddressableTools<Shop.Room.View>.LoadAllFromStringLabel(
                    "ShopView"
                )
            )
            {
                if (pair.Asset.HashCode == coreModel.HashCode)
                {
                    shopRooms.Add(pair.Asset);
                }
            }

            foreach (
                AssetWithLocation<Inventory.Room.View> pair in AddressableTools<Inventory.Room.View>.LoadAllFromStringLabel(
                    "InventoryView"
                )
            )
            {
                if (pair.Asset.HashCode == coreModel.HashCode)
                {
                    inventoryViews.Add(pair.Asset);
                }
            }

            foreach (
                AssetWithLocation<TileUnion.TileUnionImpl> pair in AddressableTools<TileUnion.TileUnionImpl>.LoadAllFromStringLabel(
                    "TileUnion"
                )
            )
            {
                if (pair.Asset.HashCode == coreModel.HashCode)
                {
                    tileUnions.Add(pair.Asset);
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
