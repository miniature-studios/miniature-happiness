#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Level
{
    [CustomEditor(typeof(InventoryController))]
    public class InventoryControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            InventoryController inventory_controller =
                serializedObject.targetObject as InventoryController;

            foreach (NamedRoomInventoryUI item in inventory_controller.NamedRoomInventoryUIs)
            {
                _ = EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(item.Name))
                {
                    inventory_controller.AddNewRoom(item.RoomInventoryUI);
                }
                EditorGUILayout.EndHorizontal();
            }
            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add gods inventory"))
            {
                foreach (NamedRoomInventoryUI item in inventory_controller.NamedRoomInventoryUIs)
                {
                    for (int i = 0; i < 666; i++)
                    {
                        inventory_controller.AddNewRoom(item.RoomInventoryUI);
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
