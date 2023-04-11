using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileBuilderController))]
class TileBuilderControllerEditor : Editor
{
    string LoadPath = "/Saves/Random1.txt";
    string SavePath = "/Saves/Random1.txt";
    public override void OnInspectorGUI()
    {
        
        var controller = serializedObject.targetObject as TileBuilderController;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Save path: ");
        SavePath = EditorGUILayout.TextField(SavePath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save scene into file."))
        {
            string path = Application.dataPath + SavePath;
            controller.tileBuilder.SaveSceneComposition(path);
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
            controller.tileBuilder.LoadSceneComposition(path);
        }
        EditorGUILayout.EndHorizontal();

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}

