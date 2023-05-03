#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public partial class TileBuilderControllerInspector
{
    public partial void ShowAddingUIButton(TileBuilderController tileBuilderController)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add UIelement"))
        {
            tileBuilderController.CreateUIElement(tileBuilderController.TileToCreatePrefab);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
