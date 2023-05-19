#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public partial class TileBuilderInspector
{
    private readonly int squareSideLength = 30;
    public partial void ShowLocationBuildingButtons(TileBuilder tile_builder)
    {
        _ = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create random building"))
        {
            int x = 0;
            int y = 0;
            tile_builder.DeleteAllTiles();
            for (int i = 0; i < squareSideLength * squareSideLength; i++)
            {
                float value = Random.value * 100;
                if (value < 50)
                {
                    tile_builder.CreateTileAndBind(tile_builder.FreespacePrefab, new(x, y), 0);
                }
                else if (value is > 50 and < 65)
                {
                    tile_builder.CreateTileAndBind(tile_builder.StairsPrefab, new(x, y), 0);
                }
                else if (value is > 65 and < 80)
                {
                    tile_builder.CreateTileAndBind(tile_builder.WindowPrefab, new(x, y), 0);
                }
                else if (value > 80)
                {
                    tile_builder.CreateTileAndBind(tile_builder.OutdoorPrefab, new(x, y), 0);
                }
                y++;
                if (y >= squareSideLength)
                {
                    y = 0;
                    x++;
                }
            }
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
            tile_builder.DeleteAllTiles();
            tile_builder.CreateTileAndBind(tile_builder.OutdoorPrefab, new(0, 0), 0);
            tile_builder.CreateTileAndBind(tile_builder.OutdoorPrefab, new(0, 1), 0);
            tile_builder.CreateTileAndBind(tile_builder.WorkingPlaceFree, new(1, 0), 0);
            tile_builder.CreateTileAndBind(tile_builder.WorkingPlace, new(1, 1), 0);
        }
        if (GUILayout.Button("Update All"))
        {
            tile_builder.UpdateAllTiles();
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif