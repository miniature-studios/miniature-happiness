using System;
using UnityEngine;
using UnityEditor;
using Common;

[CustomEditor(typeof(TileBuilder))]
public class TileBuilderEditor : Editor
{
    [SerializeField] Gamemode gamemode;

    [SerializeField] GameObject buildPrefab;
    [SerializeField] GameObject stairsPrefab;
    [SerializeField] GameObject windowPrefab;
    [SerializeField] GameObject outdoorPrefab;

    public override void OnInspectorGUI()
    {
        var tileBuilder = serializedObject.targetObject as TileBuilder;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Change game mode"))
        {
            tileBuilder.ChangeGameMode(gamemode);
        }
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

