using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using UnityEditor;

[Serializable]
public class TileInfo
{
    public GameObject prefab;
    public Vector2Int position;
    public int rotation;
    public TileInfo(GameObject prefab, Vector2Int position, int rotation)
    {
        this.prefab = prefab;
        this.position = position;
        this.rotation = rotation;
    }
}
public class TileBuilder
{
    Transform creating_transform;
    GameObject pointer_prefab;

    int Y_max_matrix_placing = 20;
    int X_matrix_placing = 0;
    Tile SelectedTile = null;
    GameObject pointer;
    List<Tile> AllTiles = new();
    Vector2Int previous_place;
    int previous_rotation;

    public TileBuilder(GameObject pointer_prefab, Transform creating_transform )
    {
        this.pointer_prefab = pointer_prefab;
        this.creating_transform = creating_transform;
    }
    public void CreateRandomTiles(GameObject buildPrefab, GameObject stairsPrefab, GameObject windowPrefab, GameObject outdoorPrefab)
    {
        for (int i = 0; i < Y_max_matrix_placing * Y_max_matrix_placing; i++)
        {
            var value = UnityEngine.Random.value * 100;
            if (value < 50)
            {
                AddTileToScene(buildPrefab);
            }
            else if(value > 50 && value < 65)
            {
                AddTileToScene(stairsPrefab);
            }
            else if (value > 65 && value < 80)
            {
                AddTileToScene(windowPrefab);
            }
            else if (value > 80)
            {
                AddTileToScene(outdoorPrefab);
            }
        }
    }
    public void ChangeXMatrixPlacing(int value)
    {
        X_matrix_placing += value;
    }
    public void LoadSceneComposition(string file_path)
    {
        while (AllTiles.Count > 0)
            DeleteTile(AllTiles.Last());
        var infos = JsonConvert.DeserializeObject<List<TileInfo>>(File.ReadAllText(file_path));
        foreach (var tile in infos)
        {
            CreateTile(tile.prefab, tile.position, tile.rotation);
        }
    }
    public void SaveSceneComposition(string file_path)
    {
        File.WriteAllText(file_path, JsonConvert.SerializeObject(AllTiles.Select(x => new TileInfo(x.gameObject, x.Position, x.Rotation)).ToList()));
    }
    public void SelectTile(Tile tile)
    {
        if (AllTiles.Contains(tile) && SelectedTile != tile)
        {
            ComletePlacing();
            SelectedTile = tile;
            previous_place = SelectedTile.Position;
            previous_rotation = SelectedTile.Rotation;
            pointer = GameObject.Instantiate(pointer_prefab, tile.transform.position, new Quaternion());
        }
    }
    public void ComletePlacing()
    {
        if (SelectedTile == null)
            return;
        if (AllTiles.Where(x => x != SelectedTile).Select(x => x.Position).Contains(SelectedTile.Position)) 
        {
            SelectedTile.Position = previous_place;
            SelectedTile.Rotation = previous_rotation;
        }
        else
        {
            UpdateTilesSides(SelectedTile.Position);
            UpdateTilesSides(previous_place);
        }
        SelectedTile = null;
        GameObject.Destroy(pointer.gameObject);
    }
    public void AddTileToScene(GameObject tilePrefab)
    {
        var new_position = new Vector2Int(X_matrix_placing, 0);
        while (AllTiles.Select(x => x.Position).Contains(new_position))
        {
            new_position += Direction.Up.ToVector2Int();
            if (new_position.y >= Y_max_matrix_placing)
            {
                X_matrix_placing++;
                new_position.x++;
                new_position.y = 0;
            }
        }
        CreateTile(tilePrefab, new_position, 0);
    }
    public Tile CreateTile(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        var tile = GameObject.Instantiate(tilePrefab, creating_transform).GetComponent<Tile>();
        AllTiles.Add(tile);
        tile.Position = position;
        tile.Rotation = rotation;
        UpdateTilesSides(tile.Position);
        return tile;
    }
    public void DeleteSelectedTile()
    {
        Tile buffer = SelectedTile;
        ComletePlacing();
        DeleteTile(buffer);
    }
    void DeleteTile(Tile tile)
    {
        AllTiles.Remove(tile);
        Vector2Int clear_position = tile.Position;
        GameObject.Destroy(tile.gameObject);
        UpdateTilesSides(clear_position);
    }
    public void UpdateTilesSides(Vector2Int position)
    {
        for (int i = 1; i >= -1; i--)
        {
            for (int j = -1; j <= 1; j++)
            {
                UpdateWallsInPosition(new Vector2Int(position.x + j, position.y + i));
            }
        }
        for (int i = 1; i >= -1; i--)
        {
            for (int j = -1; j <= 1; j++)
            {
                UpdateCornersInPosition(new Vector2Int(position.x + j, position.y + i));
            }
        }
    }
    void UpdateWallsInPosition(Vector2Int position)
    {
        var tile = AllTiles.FirstOrDefault(x => x.Position == position);
        if (tile == null)
            return;
        List<Tile> tiles_around = new();
        for (int i = 1; i >= -1; i--)
        {
            for (int j = -1; j <= 1; j++)
            {
                var buffer = AllTiles.FirstOrDefault(x => x.Position == new Vector2Int(tile.Position.x + j, tile.Position.y + i));
                if (buffer != null)
                {
                    tiles_around.Add(buffer);
                }
                else
                {
                    tiles_around.Add(null);
                }
            }
        }
        tile.UpdateWalls(tiles_around);
    }
    void UpdateCornersInPosition(Vector2Int position)
    {
        var tile = AllTiles.FirstOrDefault(x => x.Position == position);
        if (tile == null)
            return;
        List<Tile> tiles_around = new();
        for (int i = 1; i >= -1; i--)
        {
            for (int j = -1; j <= 1; j++)
            {
                var buffer = AllTiles.FirstOrDefault(x => x.Position == new Vector2Int(tile.Position.x + j, tile.Position.y + i));
                if (buffer != null)
                {
                    tiles_around.Add(buffer);
                }
                else
                {
                    tiles_around.Add(null);
                }
            }
        }
        tile.UpdateCorners(tiles_around);
    }
    public bool IsTileSelected()
    {
        return SelectedTile != null;
    }
    public void MoveSelectedTile(Direction direction)
    {
        SelectedTile.Move(direction);
        pointer.transform.position = SelectedTile.transform.position;
    }
    public void RotateSelectedTile()
    {
        SelectedTile.RotateRight();
    }
}
