using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public partial class TileBuilderEditor
{
    GameObject buildPrefab;
    GameObject stairsPrefab;
    GameObject windowPrefab;
    GameObject outdoorPrefab;
    public partial void ShowEditorRandomTilesCreating(TileBuilder tileBuilder)
    {
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
    }
}
