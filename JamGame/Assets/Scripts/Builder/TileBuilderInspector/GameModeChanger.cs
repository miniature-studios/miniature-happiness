using Common;
using UnityEditor;
using UnityEngine;

public partial class TileBuilderInspector
{
    Gamemode gamemode;
    public partial void ShowGameModeChangeing(TileBuilder tileBuilder)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("New Gamemode: ");
        gamemode = (Gamemode)EditorGUILayout.EnumPopup(gamemode);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Change game mode"))
        {
            tileBuilder.ChangeGameMode(gamemode);
        }
        EditorGUILayout.EndHorizontal();
    }
}

