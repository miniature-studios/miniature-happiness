using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuilderValidator))]
class BuilderValidatorEditor : Editor
{
    string LoadPath = "/Saves/Random1.txt";
    string SavePath = "/Saves/Random1.txt";
    public override void OnInspectorGUI()
    {
        var validator = serializedObject.targetObject as BuilderValidator;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Save path: ");
        SavePath = EditorGUILayout.TextField(SavePath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save scene into file."))
        {
            string path = Application.dataPath + SavePath;
            validator.tileBuilder.SaveSceneComposition(path);
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
            validator.tileBuilder.LoadSceneComposition(path);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Turn ON Editor Mode."))
        {
            validator.SetEditorMode(true);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Turn OFF Editor Mode."))
        {
            validator.SetEditorMode(false);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Turn ON Game Mode."))
        {
            validator.SetGameMode(true);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Turn OFF Game Mode."))
        {
            validator.SetGameMode(false);
        }
        EditorGUILayout.EndHorizontal();

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}

