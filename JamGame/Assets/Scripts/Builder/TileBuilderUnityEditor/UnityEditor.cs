using System;
using UnityEditor;

[CustomEditor(typeof(TileBuilder))]
public partial class TileBuilderUnityEditor : Editor
{
    public partial void ShowEditorGameModeChangeing(TileBuilder tileBuilder);
    public partial void ShowEditorRandomTilesCreating(TileBuilder tileBuilder);
    public partial void ShowEditorTilesSaveLoading(TileBuilder tileBuilder);
    public override void OnInspectorGUI()
    {
        var tileBuilder = serializedObject.targetObject as TileBuilder;

        ShowEditorGameModeChangeing(tileBuilder);
        ShowEditorRandomTilesCreating(tileBuilder);
        ShowEditorTilesSaveLoading(tileBuilder);

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}

