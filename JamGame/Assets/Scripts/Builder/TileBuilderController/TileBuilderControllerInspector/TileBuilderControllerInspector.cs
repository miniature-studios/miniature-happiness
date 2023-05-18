#if UNITY_EDITOR
using Common;
using UnityEditor;

[CustomEditor(typeof(TileBuilderController))]
public partial class TileBuilderControllerInspector : Editor
{
    private Gamemode GamemodeToChange = Gamemode.godmode;
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
