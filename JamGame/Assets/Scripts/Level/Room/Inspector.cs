using Common;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
            roomInformation = GetComponent<RoomInformation>();
            shopModel = GetComponent<Shop.Room.Model>();
            inventoryModel = GetComponent<Inventory.Room.Model>();
            tileUnionModel = GetComponent<TileUnion.Model>();
        }
    }

#if UNITY_EDITOR
    [CanEditMultipleObjects]
    [CustomEditor(typeof(CoreModel))]
    public class CoreModelInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            CoreModel coreModel = serializedObject.targetObject as CoreModel;

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set new Hash Code"))
            {
                coreModel.SetHashCode();
                EditorUtility.SetDirty(coreModel);
            }
            EditorGUILayout.EndHorizontal();

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
                LocationLinkPair<Shop.Room.View> pair in AddressablesTools.LoadAllFromLabel<Shop.Room.View>(
                    "ShopView"
                )
            )
            {
                if (pair.Link.CoreModel.HashCode == coreModel.HashCode)
                {
                    shopRooms.Add(pair.Link);
                }
            }

            foreach (
                LocationLinkPair<Inventory.Room.View> pair in AddressablesTools.LoadAllFromLabel<Inventory.Room.View>(
                    "InventoryView"
                )
            )
            {
                if (pair.Link.CoreModel.HashCode == coreModel.HashCode)
                {
                    inventoryViews.Add(pair.Link);
                }
            }

            foreach (
                LocationLinkPair<TileUnion.TileUnionImpl> pair in AddressablesTools.LoadAllFromLabel<TileUnion.TileUnionImpl>(
                    "TileUnion"
                )
            )
            {
                if (pair.Link.CoreModel.HashCode == coreModel.HashCode)
                {
                    tileUnions.Add(pair.Link);
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
#endif
}
