using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Tile))]
public class TileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var tile = serializedObject.targetObject as Tile;

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Fill tile with prefabs"))
        {
            tile.UpdateTilePrefabComponents();
        }

        EditorGUILayout.EndHorizontal();

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}

