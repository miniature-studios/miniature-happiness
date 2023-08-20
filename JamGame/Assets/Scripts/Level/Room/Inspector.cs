#if UNITY_EDITOR
using Common;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Level.Room
{
    [CustomEditor(typeof(CoreModel))]
    public class CoreModelInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            CoreModel coreModel = serializedObject.targetObject as CoreModel;

            _ = EditorGUILayout.BeginHorizontal();
            if (
                GUILayout.Button("Update ID")
                && EditorUtility.DisplayDialog(
                    "Are you sure to Update ID?",
                    $"Are you sure you want Update Id on {coreModel.gameObject.name} Core model?",
                    "Update",
                    "Do Not Update"
                )
            )
            {
                coreModel.GenerateId();
                EditorUtility.SetDirty(coreModel.gameObject);
                AssetDatabase.SaveAssets();
            }
            EditorGUILayout.EndHorizontal();

            List<Shop.Room.View> shopRooms = new();
            List<Inventory.Room.View> inventoryViews = new();
            List<TileUnion.TileUnionImpl> tileUnions = new();

            foreach (GameObject prefab in PrefabsTools.GetAllAssetsPrefabs())
            {
                Shop.Room.View shopView = prefab.GetComponent<Shop.Room.View>();
                if (shopView != null && shopView.UniqueId == coreModel.UniqueId)
                {
                    shopRooms.Add(prefab.GetComponent<Shop.Room.View>());
                }

                Inventory.Room.View inventoryView = prefab.GetComponent<Inventory.Room.View>();
                if (inventoryView != null && inventoryView.UniqueId == coreModel.UniqueId)
                {
                    inventoryViews.Add(prefab.GetComponent<Inventory.Room.View>());
                }

                TileUnion.TileUnionImpl tileUnionImpl =
                    prefab.GetComponent<TileUnion.TileUnionImpl>();
                if (tileUnionImpl != null && tileUnionImpl.UniqueId == coreModel.UniqueId)
                {
                    tileUnions.Add(prefab.GetComponent<TileUnion.TileUnionImpl>());
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
