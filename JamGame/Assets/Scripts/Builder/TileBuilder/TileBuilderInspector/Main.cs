#if UNITY_EDITOR
using System;
using UnityEditor;

[CustomEditor(typeof(TileBuilder))]
public partial class TileBuilderInspector : Editor
{
    public partial void ShowGameModeChangeing(TileBuilder tileBuilder);
    public partial void ShowLocationBuildingButtons(TileBuilder tileBuilder);
    public partial void ShowTilesSaveLoading(TileBuilder tileBuilder);
    public override void OnInspectorGUI()
    {
        var tileBuilder = serializedObject.targetObject as TileBuilder;

        ShowGameModeChangeing(tileBuilder);
        ShowLocationBuildingButtons(tileBuilder);
        ShowTilesSaveLoading(tileBuilder);

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}

#endif