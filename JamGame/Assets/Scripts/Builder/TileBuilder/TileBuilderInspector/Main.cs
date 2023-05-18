#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TileBuilder))]
public partial class TileBuilderInspector : Editor
{
    public partial void ShowLocationBuildingButtons(TileBuilder tileBuilder);
    public partial void ShowTilesSaveLoading(TileBuilder tileBuilder);
    public override void OnInspectorGUI()
    {
        TileBuilder tileBuilder = serializedObject.targetObject as TileBuilder;

        ShowLocationBuildingButtons(tileBuilder);
        ShowTilesSaveLoading(tileBuilder);

        _ = DrawDefaultInspector();

        _ = serializedObject.ApplyModifiedProperties();
    }
}

#endif