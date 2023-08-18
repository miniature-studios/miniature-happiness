#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Level.Inventory.Inspector
{
    [CustomEditor(typeof(Controller))]
    public class ControllerEditor : Editor
    {
        private Level.Room.CoreModel room;

        public override void OnInspectorGUI()
        {
            Controller inventory_controller = serializedObject.targetObject as Controller;

            _ = EditorGUILayout.BeginHorizontal();
            room = (Level.Room.CoreModel)
                EditorGUILayout.ObjectField(
                    "Select inv room to add: ",
                    room,
                    typeof(Level.Room.CoreModel),
                    false
                );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add selected room"))
            {
                inventory_controller.AddNewRoom(room);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add rooms from folder."))
            {
                List<string> paths = Directory
                    .GetFiles("Assets/Prefabs/UI/InventoryUI/TileUnionsUI")
                    .ToList();

                foreach (
                    string item in Directory.GetDirectories(
                        "Assets/Prefabs/UI/InventoryUI/TileUnionsUI/"
                    )
                )
                {
                    paths.AddRange(Directory.GetFiles(item));
                }

                foreach (
                    string path in paths
                        .Where(x => !x.EndsWith("meta"))
                        .Select(x => x.Replace($"\\", "/"))
                )
                {
                    Object inv_el = AssetDatabase.LoadAssetAtPath(path, typeof(Level.Room.CoreModel));
                    for (int i = 0; i < 50; i++)
                    {
                        inventory_controller.AddNewRoom(inv_el as Level.Room.CoreModel);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
