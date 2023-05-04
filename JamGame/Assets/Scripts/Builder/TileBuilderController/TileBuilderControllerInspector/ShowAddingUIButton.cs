#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public partial class TileBuilderControllerInspector
{
    public partial void ShowAddingUIButton(TileBuilderController tileBuilderController)
    {
        _ = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add UIelement"))
        {
            _ = tileBuilderController.CreateUIElement(tileBuilderController.TileToCreatePrefab);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
