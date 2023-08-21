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
                foreach (GameObject prefab in PrefabsTools.GetAllAssetsPrefabs())
                {
                    CoreModel coreModel = prefab.GetComponent<CoreModel>();
                    if (coreModel != null)
                    {
                        for (int i = 0; i < 50; i++)
                        {
                            inventory_controller.AddNewRoom(coreModel);
                        }
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
