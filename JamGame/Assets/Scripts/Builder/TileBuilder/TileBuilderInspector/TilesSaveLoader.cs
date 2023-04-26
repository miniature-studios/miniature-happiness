#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public partial class TileBuilderInspector
{
    public partial void ShowTilesSaveLoading(TileBuilder tileBuilder)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save scene into file."))
        {
            string path = Application.dataPath + tileBuilder.SavePath;
            tileBuilder.SaveSceneComposition(path);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load scene from file."))
        {
            string path = Application.dataPath + tileBuilder.LoadPath;
            tileBuilder.LoadSceneComposition(path);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
