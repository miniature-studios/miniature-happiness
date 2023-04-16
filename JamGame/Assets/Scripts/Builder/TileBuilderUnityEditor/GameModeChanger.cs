using Common;
using UnityEditor;
using UnityEngine;

public partial class TileBuilderEditor : Editor
{
    Gamemode gamemode;
    public partial void ShowEditorGameModeChangeing(TileBuilder tileBuilder)
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

