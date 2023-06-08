#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public partial class TileBuilderInspector
{
    public partial void ShowLocationBuildingButtons(TileBuilder tile_builder)
    {
        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.SquareSideLength = EditorGUILayout.IntField(
            "Square side length: ",
            tile_builder.SquareSideLength
        );
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.StairsPrefab = (TileUnion)
            EditorGUILayout.ObjectField(
                "Stairs prefab: ",
                tile_builder.StairsPrefab,
                typeof(TileUnion),
                true
            );
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.WindowPrefab = (TileUnion)
            EditorGUILayout.ObjectField(
                "Window prefab: ",
                tile_builder.WindowPrefab,
                typeof(TileUnion),
                true
            );
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.OutdoorPrefab = (TileUnion)
            EditorGUILayout.ObjectField(
                "Outdoor prefab: ",
                tile_builder.OutdoorPrefab,
                typeof(TileUnion),
                true
            );
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.CorridoorPrefab = (TileUnion)
            EditorGUILayout.ObjectField(
                "Corridoor prefab: ",
                tile_builder.CorridoorPrefab,
                typeof(TileUnion),
                true
            );
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.WorkingPlaceFree = (TileUnion)
            EditorGUILayout.ObjectField(
                "Working place free prefab: ",
                tile_builder.WorkingPlaceFree,
                typeof(TileUnion),
                true
            );
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.WorkingPlace = (TileUnion)
            EditorGUILayout.ObjectField(
                "Working place prefab: ",
                tile_builder.WorkingPlace,
                typeof(TileUnion),
                true
            );
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
