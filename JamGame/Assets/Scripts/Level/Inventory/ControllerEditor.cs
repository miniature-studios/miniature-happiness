#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Level.Inventory.Inspector
{
    [CustomEditor(typeof(Controller))]
    public class ControllerEditor : Editor
    {
        private Room.Model room;

        public override void OnInspectorGUI()
        {
            Controller inventory_controller = serializedObject.targetObject as Controller;

            _ = EditorGUILayout.BeginHorizontal();
            room = (Room.Model)
                EditorGUILayout.ObjectField(
                    "Select inv room to add: ",
                    room,
                    typeof(Room.Model),
                    false
                );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add selected room"))
            {
                inventory_controller.AddNewRoom(room);
            }
            EditorGUILayout.EndHorizontal();

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
