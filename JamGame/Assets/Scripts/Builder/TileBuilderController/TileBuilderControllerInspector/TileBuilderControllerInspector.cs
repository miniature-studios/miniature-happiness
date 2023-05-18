#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TileBuilderController))]
public partial class TileBuilderControllerInspector : Editor
{
    public partial void ShowGameModeChangeing(TileBuilderController tileBuilder);
    public override void OnInspectorGUI()
    {
        TileBuilderController tileBuilderController = serializedObject.targetObject as TileBuilderController;

        ShowGameModeChangeing(tileBuilderController);

        _ = DrawDefaultInspector();

        _ = serializedObject.ApplyModifiedProperties();
    }
}

#endif
