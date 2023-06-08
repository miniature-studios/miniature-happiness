#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TileBuilder))]
public partial class TileBuilderInspector : Editor
{
    public partial void ShowLocationBuildingButtons(TileBuilder tile_builder);

    public partial void ShowTilesSaveLoading(TileBuilder tile_builder);

    public override void OnInspectorGUI()
    {
        TileBuilder tile_builder = serializedObject.targetObject as TileBuilder;

        ShowLocationBuildingButtons(tile_builder);
        ShowTilesSaveLoading(tile_builder);

        _ = DrawDefaultInspector();

        _ = serializedObject.ApplyModifiedProperties();
    }
}

#endif
