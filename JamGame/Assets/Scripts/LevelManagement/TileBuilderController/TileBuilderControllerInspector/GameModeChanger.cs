#if UNITY_EDITOR
using Common;
using UnityEditor;
using UnityEngine;

public partial class TileBuilderControllerInspector
{
    private Gamemode gamemode_to_change = Gamemode.God;
    public partial void ShowGameModeChangeing(TileBuilderController tile_builder)
    {
        _ = EditorGUILayout.BeginHorizontal();
        gamemode_to_change = (Gamemode)EditorGUILayout.EnumPopup("Gamemode To Change:", gamemode_to_change);
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Change game mode"))
        {
            tile_builder.ChangeGameMode(gamemode_to_change);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
