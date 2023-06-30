#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TileBuilder.ControllerInspector
{
    [CustomEditor(typeof(Controller))]
    public class Inspector : Editor
    {
        private GameMode gamemode_to_change = GameMode.God;

        public void DisplayGameModeChange(Controller tile_builder)
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
            Controller tile_builder_controller = serializedObject.targetObject as Controller;

            DisplayGameModeChange(tile_builder_controller);

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
