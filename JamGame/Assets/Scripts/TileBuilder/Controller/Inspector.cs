#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TileBuilder.Controller.Inspector
{
    [CustomEditor(typeof(TileBuilderController))]
    public class Inspector : Editor
    {
        private GameMode gamemode_to_change = GameMode.God;

        public void DisplayGameModeChange(TileBuilderController tile_builder)
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
            TileBuilderController tile_builder_controller =
                serializedObject.targetObject as TileBuilderController;

            DisplayGameModeChange(tile_builder_controller);

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
