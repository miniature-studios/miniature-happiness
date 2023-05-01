#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public partial class TileBuilderInspector
{
    int squareSideLength = 2;
    public partial void ShowLocationBuildingButtons(TileBuilder tileBuilder)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Create random building"))
        {
            int x = 0;
            int y = 0;
            tileBuilder.DeleteAllTiles();
            for (int i = 0; i < squareSideLength * squareSideLength; i++)
            {
                var value = Random.value * 100;
                if (value < 50)
                {
                    tileBuilder.CreateTile(tileBuilder.FreespacePrefab, new(x,y), 0);
                }
                else if (value > 50 && value < 65)
                {
                    tileBuilder.CreateTile(tileBuilder.StairsPrefab, new(x, y), 0);
                }
                else if (value > 65 && value < 80)
                {
                    tileBuilder.CreateTile(tileBuilder.WindowPrefab, new(x, y), 0);
                }
                else if (value > 80)
                {
                    tileBuilder.CreateTile(tileBuilder.OutdoorPrefab, new(x, y), 0);
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
            tileBuilder.DeleteAllTiles();
            for (int i = 0; i < 9; i++)
                tileBuilder.CreateTile(tileBuilder.OutdoorPrefab, new(0, i), 0);
            for (int i = 0; i < 8; i++)
            {
                if (i == 1)
                    tileBuilder.CreateTile(tileBuilder.StairsPrefab, new(i + 1, 0), 0);
                else
                    tileBuilder.CreateTile(tileBuilder.OutdoorPrefab, new(i + 1, 0), 0);
                for (int j = 0; j < 7; j++)
                {
                    if(j == 2) 
                        tileBuilder.CreateTile(tileBuilder.CorridoorPrefab, new(i + 1, j + 1), 0);
                    else if(j == 3)
                        tileBuilder.CreateTile(tileBuilder.WorkingPlace, new(i + 1, j + 1), 0);
                    else if (j == 4)
                        tileBuilder.CreateTile(tileBuilder.WorkingPlaceFree, new(i + 1, j + 1), 0);
                    else
                        tileBuilder.CreateTile(tileBuilder.FreespacePrefab, new(i + 1, j + 1), 0);
                }
                tileBuilder.CreateTile(tileBuilder.OutdoorPrefab, new(i + 1, 8), 0);
            }
            for (int i = 0; i < 9; i++)
                tileBuilder.CreateTile(tileBuilder.OutdoorPrefab, new(9, i), 0);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add choosed Tile"))
        {
            tileBuilder.Execute(new AddTileToSceneCommand(tileBuilder, tileBuilder.ChoosedTile));
        }
        if (GUILayout.Button("Clear Scene"))
        {
            tileBuilder.DeleteAllTiles();
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Add 4 Tiles"))
        {
            tileBuilder.DeleteAllTiles();
            tileBuilder.CreateTile(tileBuilder.OutdoorPrefab, new(0, 0), 0);
            tileBuilder.CreateTile(tileBuilder.OutdoorPrefab, new(0, 1), 0);
            tileBuilder.CreateTile(tileBuilder.WorkingPlaceFree, new(1, 0), 0);
            tileBuilder.CreateTile(tileBuilder.WorkingPlace, new(1, 1), 0);
        }
        if (GUILayout.Button("Update All"))
        {
            tileBuilder.UpdateAllTiles();
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif