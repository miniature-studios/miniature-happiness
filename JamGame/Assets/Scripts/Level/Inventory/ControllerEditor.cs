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
            Controller controller = serializedObject.targetObject as Controller;

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
                controller.AddNewRoom(
                    CoreModelsManager.Instance.InstantiateCoreModel(room.HashCode)
                );
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add rooms from folder."))
            {
                foreach (
                    AssetWithLocation<CoreModel> core in AddressableTools<CoreModel>.LoadAllFromStringLabel(
                        "CoreModel"
                    )
                )
                {
                    controller.AddNewRoom(
                        CoreModelsManager.Instance.InstantiateCoreModel(core.Asset.HashCode)
                    );
                }
            }
            EditorGUILayout.EndHorizontal();

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
