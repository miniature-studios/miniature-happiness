#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TileBuilder.Controller.Inspector
{
    [CustomEditor(typeof(ControllerImpl))]
    public class Inspector : Editor
    {
        private GameMode gamemode_to_change = GameMode.God;

        public void DisplayGameModeChange(ControllerImpl tile_builder)
        {
            _ = EditorGUILayout.BeginHorizontal();
            gamemode_to_change = (GameMode)
                EditorGUILayout.EnumPopup("Gamemode To Change:", gamemode_to_change);
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Change game mode"))
            {
                tile_builder.ChangeGameMode(gamemode_to_change);
            }
            EditorGUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            ControllerImpl tile_builder_controller =
                serializedObject.targetObject as ControllerImpl;

            DisplayGameModeChange(tile_builder_controller);

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
