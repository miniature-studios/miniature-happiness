#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public partial class TileBuilderInspector
{
    public partial void ShowGameModeChangeing(TileBuilder tileBuilder)
    {
        _ = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Change game mode"))
        {
            tileBuilder.ChangeGameMode(tileBuilder.GameMode);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
