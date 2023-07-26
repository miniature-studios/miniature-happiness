#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Level.Inventory.Inspector
{
    [CustomEditor(typeof(Controller))]
    public class ControllerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Controller inventory_controller = serializedObject.targetObject as Controller;

            foreach (NamedRoomInventoryUI item in inventory_controller.NamedRoomInventoryUIs)
            {
                _ = EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(item.Name))
                {
                    inventory_controller.AddNewRoom(item.Room);
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
                        inventory_controller.AddNewRoom(item.Room);
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
