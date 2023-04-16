using System;
using UnityEditor;

[CustomEditor(typeof(TileBuilder))]
public partial class TileBuilderInspector : Editor
{
    public partial void ShowGameModeChangeing(TileBuilder tileBuilder);
    public partial void ShowRandomTilesCreating(TileBuilder tileBuilder);
    public partial void ShowTilesSaveLoading(TileBuilder tileBuilder);
    public override void OnInspectorGUI()
    {
        var tileBuilder = serializedObject.targetObject as TileBuilder;

        ShowGameModeChangeing(tileBuilder);
        ShowRandomTilesCreating(tileBuilder);
        ShowTilesSaveLoading(tileBuilder);

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}

