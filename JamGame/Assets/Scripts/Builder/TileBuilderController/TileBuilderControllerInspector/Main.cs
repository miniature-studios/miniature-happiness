#if UNITY_EDITOR
using System;
using UnityEditor;

[CustomEditor(typeof(TileBuilderController))]
public partial class TileBuilderControllerInspector : Editor
{
    public partial void ShowAddingUIButton(TileBuilderController tileBuilder);
    public override void OnInspectorGUI()
    {
        var tileBuilderController = serializedObject.targetObject as TileBuilderController;

        ShowAddingUIButton(tileBuilderController);

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}

#endif