#if UNITY_EDITOR
using Common;
using Level.Room;
using UnityEditor;
using UnityEngine;

namespace Level.Inventory.Inspector
{
    [CustomEditor(typeof(Controller))]
    public class ControllerEditor : Editor
    {
        private CoreModel room;

        public override void OnInspectorGUI()
        {
            Controller inventory_controller = serializedObject.targetObject as Controller;

            _ = EditorGUILayout.BeginHorizontal();
            room = (CoreModel)
                EditorGUILayout.ObjectField(
                    "Select inv room to add: ",
                    room,
                    typeof(CoreModel),
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
                foreach (
                    LocationLinkPair<CoreModel> pair in AddressablesTools.LoadAllFromLabel<CoreModel>(
                        "CoreModel"
                    )
                )
                {
                    for (int i = 0; i < 50; i++)
                    {
                        inventory_controller.AddNewRoom(pair.Link);
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
