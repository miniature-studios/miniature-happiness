#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public partial class TileBuilderControllerInspector
{
    public partial void ShowGameModeChangeing(TileBuilderController tileBuilderController)
    {
        _ = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Change game mode"))
        {
            tileBuilderController.ChangeGameMode(tileBuilderController.GameMode);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
