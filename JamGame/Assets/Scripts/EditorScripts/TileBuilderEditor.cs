using System;
using UnityEngine;
using UnityEditor;
using Common;

[CustomEditor(typeof(TileBuilder))]
public class TileBuilderEditor : Editor
{
    Gamemode gamemode;

    GameObject buildPrefab;
    GameObject stairsPrefab;
    GameObject windowPrefab;
    GameObject outdoorPrefab;

    string LoadPath = "/Saves/Random1.txt";
    string SavePath = "/Saves/Random1.txt";

    public override void OnInspectorGUI()
    {
        var tileBuilder = serializedObject.targetObject as TileBuilder;

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

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("New Gamemode: ");
        gamemode = (Gamemode)EditorGUILayout.EnumPopup(gamemode);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Change game mode"))
        {
            tileBuilder.ChangeGameMode(gamemode);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("BuildPrefab: ");
        buildPrefab = (GameObject)EditorGUILayout.ObjectField(buildPrefab, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("StairsPrefab: ");
        stairsPrefab = (GameObject)EditorGUILayout.ObjectField(stairsPrefab, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("WindowPrefab: ");
        windowPrefab = (GameObject)EditorGUILayout.ObjectField(windowPrefab, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("OutdoorPrefab: ");
        outdoorPrefab = (GameObject)EditorGUILayout.ObjectField(outdoorPrefab, typeof(GameObject), true);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create random tiles"))
        {
            for (int i = 0; i < tileBuilder.Y_max_matrix_placing * tileBuilder.Y_max_matrix_placing; i++)
            {
                var value = UnityEngine.Random.value * 100;
                if (value < 50)
                {
                    tileBuilder.AddTileToScene(buildPrefab);
                }
                else if (value > 50 && value < 65)
                {
                    tileBuilder.AddTileToScene(stairsPrefab);
                }
                else if (value > 65 && value < 80)
                {
                    tileBuilder.AddTileToScene(windowPrefab);
                }
                else if (value > 80)
                {
                    tileBuilder.AddTileToScene(outdoorPrefab);
                }
            }
        }
        EditorGUILayout.EndHorizontal();

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}

