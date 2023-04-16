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
public class TileBuilder : MonoBehaviour
{
    [SerializeField] GameObject pointer_prefab;

    IValidator validator = new GameModeValidator();

    public int Y_max_matrix_placing = 20;
    public int X_matrix_placing = 0;
    Tile SelectedTile = null;
    GameObject pointer;
    List<Tile> AllTiles = new();
    Vector2Int previous_place;
    int previous_rotation;

    public void DoCommand(ICommand command)
    {
        if (validator.ValidateCommand(command))
        {
            // TODO do command
        }
    }
    public void ChangeGameMode(Gamemode gamemode)
    {
        switch (gamemode)
        {
            case Gamemode.godmode_outside:
                validator = new GodModeOutsideValidator();
                break;
            case Gamemode.godmode_inside:
                validator = new GodModeInsideValidator();
                break;
            case Gamemode.building:
                validator = new BuildModeValidator();
                break;
            case Gamemode.gameing:
                validator = new GameModeValidator();
                break;
            default:
                throw new ArgumentException();
        }
    }

    void ChangeXMatrixPlacing(int value)
    {
        X_matrix_placing += value;
    }
    // Public for Unity Editor in inspector
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
    void SelectTile(Tile tile)
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
    void ComletePlacing()
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
    Tile CreateTile(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        var tile = GameObject.Instantiate(tilePrefab, transform).GetComponent<Tile>();
        AllTiles.Add(tile);
        tile.Position = position;
        tile.Rotation = rotation;
        UpdateTilesSides(tile.Position);
        return tile;
    }
    void DeleteSelectedTile()
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
    void UpdateTilesSides(Vector2Int position)
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
    void MoveSelectedTile(Direction direction)
    {
        SelectedTile.Move(direction);
        pointer.transform.position = SelectedTile.transform.position;
    }
    void RotateSelectedTile()
    {
        SelectedTile.RotateRight();
    }
}
