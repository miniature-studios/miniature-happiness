#if UNITY_EDITOR
using Common;
using UnityEditor;
using UnityEngine;

public partial class TileBuilderControllerInspector
{
    public partial void ShowGameModeChangeing(TileBuilderController tileBuilderController)
    {
        _ = EditorGUILayout.BeginHorizontal();
        GamemodeToChange = (Gamemode)EditorGUILayout.EnumPopup("Gamemode To Change:", GamemodeToChange);
        EditorGUILayout.EndHorizontal();
        _ = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Change game mode"))
        {
            tileBuilderController.ChangeGameMode(GamemodeToChange);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
