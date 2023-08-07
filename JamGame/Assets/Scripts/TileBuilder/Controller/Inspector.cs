#if UNITY_EDITOR
using TileUnion;
using UnityEditor;
using UnityEngine;

namespace TileBuilder.ControllerInspector
{
    [CustomEditor(typeof(Controller))]
    public class Inspector : Editor
    {
        private GameMode game_mode_to_change = GameMode.God;
        private TileUnionImpl unionImpl_to_create = null;

        public void DisplayGameModeChange(Controller tile_builder_controller)
        {
            _ = EditorGUILayout.BeginHorizontal();
            game_mode_to_change = (GameMode)
                EditorGUILayout.EnumPopup("Game mode To Change:", game_mode_to_change);
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Change game mode"))
            {
                tile_builder_controller.ChangeGameMode(game_mode_to_change);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            unionImpl_to_create = (TileUnionImpl)
                EditorGUILayout.ObjectField(
                    "Select TileUnion to add: ",
                    unionImpl_to_create,
                    typeof(TileUnionImpl),
                    false
                );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add selected TileUnion"))
            {
                _ = tile_builder_controller.Execute(
                    new Command.AddTileToScene(
                        unionImpl_to_create,
                        tile_builder_controller.SelectedTileWrapper
                    )
                );
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
