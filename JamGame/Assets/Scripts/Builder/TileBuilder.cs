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
    public TileType tileType;
    public Vector2Int position;
    public int rotation;
    public TileInfo(TileType tileType, Vector2Int position, int rotation)
    {
        this.tileType = tileType;
        this.position = position;
        this.rotation = rotation;
    }
}
public class TileBuilder
{
    IBuilderValidator validator;
    Transform creating_transform;
    TilePrefabsHandler tilePrefabsHandler;
    GameObject pointer_prefab;

    int X_matrix_placing = 0;
    Tile SelectedTile = null;
    GameObject pointer;
    List<Tile> AllTiles = new();
    Vector2Int previous_place;
    int previous_rotation;

    public TileBuilder(TilePrefabsHandler prefabsHandler, GameObject pointer_prefab, Transform creating_transform, IBuilderValidator validator)
    {
        tilePrefabsHandler = prefabsHandler;
        this.pointer_prefab = pointer_prefab;
        this.creating_transform = creating_transform;
        this.validator = validator;
        this.validator.Init(this);
    }
    public void ChangeValidator(IBuilderValidator validator)
    {
        this.validator = validator;
        this.validator.Init(this);
    }
    public void CreateRandomTiles()
    {
        for (int i = 0; i < 100; i++)
        {
            var value = UnityEngine.Random.value * 100;
            if (value < 25)
            {
                AddTileToScene(TileType.build);
            }
            else if(value > 25 && value < 50)
            {
                AddTileToScene(TileType.stairs);
            }
            else if (value > 50 && value < 75)
            {
                AddTileToScene(TileType.window);
            }
            else if (value > 75)
            {
                AddTileToScene(TileType.outdoor);
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
            CreateTile(tile.tileType, tile.position, tile.rotation);
        }
    }
    public void SaveSceneComposition(string file_path)
    {
        File.WriteAllText(file_path, JsonConvert.SerializeObject(AllTiles.Select(x => new TileInfo(x.tileType, x.Position, x.Rotation)).ToList()));
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
    public void AddTileToScene(TileType tileType)
    {
        var new_position = new Vector2Int(X_matrix_placing, 0);
        while (AllTiles.Select(x => x.Position).Contains(new_position))
        {
            new_position += Direction.Up.ToVector2Int();
            if (new_position.y >= 10)
            {
                X_matrix_placing++;
                new_position.x++;
                new_position.y = 0;
            }
        }
        CreateTile(tileType, new_position, 0);
    }
    public Tile CreateTile(TileType tileType, Vector2Int position, int rotation)
    {
        var prefab = tilePrefabsHandler.TilePrefabHandlers.Find(x => x.Type == tileType).Prefab;
        var tile = GameObject.Instantiate(prefab, creating_transform).GetComponent<Tile>();
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
