#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public partial class TileBuilderInspector
{
    public partial void ShowLocationBuildingButtons(TileBuilder tile_builder)
    {
        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.squareSideLength = EditorGUILayout.IntField("Square side length: ", tile_builder.squareSideLength);
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.stairsPrefab = (TileUnion)EditorGUILayout.ObjectField("Stairs prefab: ", tile_builder.stairsPrefab, typeof(TileUnion), true);
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.windowPrefab = (TileUnion)EditorGUILayout.ObjectField("Window prefab: ", tile_builder.windowPrefab, typeof(TileUnion), true);
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.outdoorPrefab = (TileUnion)EditorGUILayout.ObjectField("Outdoor prefab: ", tile_builder.outdoorPrefab, typeof(TileUnion), true);
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.corridoorPrefab = (TileUnion)EditorGUILayout.ObjectField("Corridoor prefab: ", tile_builder.corridoorPrefab, typeof(TileUnion), true);
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.workingPlaceFree = (TileUnion)EditorGUILayout.ObjectField("Working place free prefab: ", tile_builder.workingPlaceFree, typeof(TileUnion), true);
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.workingPlace = (TileUnion)EditorGUILayout.ObjectField("Working place prefab: ", tile_builder.workingPlace, typeof(TileUnion), true);
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create random building"))
        {
            tile_builder.CreateRandomBuilding();
        }
        if (GUILayout.Button("Create normal building"))
        {
            tile_builder.CreateNormalBuilding();
        }
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Scene"))
        {
            tile_builder.DeleteAllTiles();
        }
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add 4 Tiles"))
        {
            tile_builder.CreateFourTiles();
        }
        if (GUILayout.Button("Update All"))
        {
            tile_builder.UpdateAllTiles();
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif