#if UNITY_EDITOR
using Common;
using UnityEditor;

[CustomEditor(typeof(TileBuilderController))]
public partial class TileBuilderControllerInspector : Editor
{
    private Gamemode gamemode_to_change = Gamemode.GodMode;

    public partial void ShowGameModeChangeing(TileBuilderController tile_builder);

    public override void OnInspectorGUI()
    {
        TileBuilderController tile_builder_controller = serializedObject.targetObject as TileBuilderController;

        ShowGameModeChangeing(tile_builder_controller);

        _ = DrawDefaultInspector();

        _ = serializedObject.ApplyModifiedProperties();
    }
}

#endif
