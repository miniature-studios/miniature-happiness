using UnityEngine;
using UnityEditor;

public partial class TileBuilderInspector
{
    string LoadPath = "/Saves/Random1.txt";
    string SavePath = "/Saves/Random1.txt";

    public partial void ShowTilesSaveLoading(TileBuilder tileBuilder)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Save path: ");
        SavePath = EditorGUILayout.TextField(SavePath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save scene into file."))
        {
            string path = Application.dataPath + SavePath;
            tileBuilder.SaveSceneComposition(path);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Load path: ");
        LoadPath = EditorGUILayout.TextField(LoadPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load scene from file."))
        {
            string path = Application.dataPath + LoadPath;
            tileBuilder.LoadSceneComposition(path);
        }
        EditorGUILayout.EndHorizontal();
    }
}

