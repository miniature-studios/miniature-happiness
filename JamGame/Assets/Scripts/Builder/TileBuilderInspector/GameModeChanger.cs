#if UNITY_EDITOR
using Common;
using UnityEditor;
using UnityEngine;

public partial class TileBuilderInspector
{
    public partial void ShowGameModeChangeing(TileBuilder tileBuilder)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Change game mode"))
        {
            tileBuilder.ChangeGameMode(tileBuilder.GameMode);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
