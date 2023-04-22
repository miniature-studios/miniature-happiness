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
    #region For Inspector
    [SerializeField] public GameObject FreecpacePrefab;
    [SerializeField] public GameObject StairsPrefab;
    [SerializeField] public GameObject WindowPrefab;
    [SerializeField] public GameObject OutdoorPrefab;
    [SerializeField] public GameObject CorridoorPrefab;
    [SerializeField] public GameObject WorkingPlaceFree;
    [SerializeField] public GameObject WorkingPlace;
    [SerializeField] public Gamemode GameMode;
    [SerializeField] public string LoadPath = "/Saves/Random1.txt";
    [SerializeField] public string SavePath = "/Saves/Random1.txt";
    #endregion

    [SerializeField] GameObject pointerPrefab;
    [SerializeField] GameObject _freecpacePrefab;

    IValidator validator;

    List<Tile> allTiles = new();

    // Buffer variables
    public Tile SelectedTile = null;
    GameObject pointer;
    Vector2Int previous_place;
    int previous_rotation;

    public bool Execute(ICommand command)
    {
        if (validator.ValidateCommand(command))
        {
            command.Execute();
            return true;
        }
        return false;
    }
    public void ChangeGameMode(Gamemode gamemode)
    {
        switch (gamemode)
        {
            case Gamemode.godmode:
                validator = new GodModeValidator(this);
                break;
            case Gamemode.building:
                validator = new BuildModeValidator(this);
                break;
            case Gamemode.gameing:
                validator = new GameModeValidator();
                break;
            default:
                throw new ArgumentException();
        }
    }

    public void LoadSceneComposition(string file_path)
    {
        DeleteAllTiles();
        var infos = JsonConvert.DeserializeObject<List<TileInfo>>(File.ReadAllText(file_path));
        foreach (var tile in infos)
        {
            CreateTile(tile.prefab, tile.position, tile.rotation);
        }
    }
    public void SaveSceneComposition(string file_path)
    {
        File.WriteAllText(file_path, JsonConvert.SerializeObject(allTiles.Select(x => new TileInfo(x.gameObject, x.Position, x.Rotation)).ToList()));
    }

    public void SelectTile(Tile tile)
    {
        if (allTiles.Contains(tile) && SelectedTile != tile)
        {
            ComletePlacing();
            SelectedTile = tile;
            ApplySelectedTile(tile);
            previous_place = SelectedTile.Position;
            previous_rotation = SelectedTile.Rotation;
            pointer = Instantiate(pointerPrefab, tile.transform.position, new Quaternion());
        }
    }
    public void ComletePlacing()
    {
        if (SelectedTile == null)
            return;
        if(previous_place == SelectedTile.Position)
        {
            UpdateTilesSides(SelectedTile.Position);
            UpdateTilesSides(previous_place);
            CancelSelectedTile(SelectedTile);
            SelectedTile = null;
            Destroy(pointer.gameObject);
            return;
        }
        if (validator is GodModeValidator)
        {
            var tileUnder = allTiles.Find(x => x != SelectedTile && x.Position == SelectedTile.Position);
            if (tileUnder == null)
            {
                UpdateTilesSides(SelectedTile.Position);
                UpdateTilesSides(previous_place);
                CancelSelectedTile(SelectedTile);
                SelectedTile = null;
                Destroy(pointer.gameObject);
                return;
            }
            if (!CheckTileForMark(tileUnder, "freecpace"))
            {
                SelectedTile.Position = previous_place;
                SelectedTile.Rotation = previous_rotation;
            }
            else
            {
                DeleteTile(tileUnder);
                CreateFreeCpace(previous_place);
                UpdateTilesSides(SelectedTile.Position);
                UpdateTilesSides(previous_place);
            }
            CancelSelectedTile(SelectedTile);
            SelectedTile = null;
            Destroy(pointer.gameObject);
        }
        else
        {
            var tileUnder = allTiles.Find(x => x != SelectedTile && x.Position == SelectedTile.Position);
            if (!CheckTileForMark(tileUnder, "freecpace"))
            {
                SelectedTile.Position = previous_place;
                SelectedTile.Rotation = previous_rotation;
            }
            else
            {
                DeleteTile(tileUnder);
                CreateFreeCpace(previous_place);
                UpdateTilesSides(SelectedTile.Position);
                UpdateTilesSides(previous_place);
            }
            CancelSelectedTile(SelectedTile);
            SelectedTile = null;
            Destroy(pointer.gameObject);
        }
    }
    public GameObject DeleteSelectedTile()
    {
        Tile buffer = SelectedTile;
        ComletePlacing();
        DeleteTile(buffer);
        return buffer.gameObject;
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
    public bool CheckTileForMark(Tile tile, string tag)
    {
        if (tile == null)
            return false;
        return tile.Marks.Contains(tag);
    }
    public List<Tile> GetTilesInPosition(Vector2Int position)
    {
        return allTiles.FindAll(t => t.Position == position);
    }
    public List<Vector2Int> GetInsideListPositions()
    {
        return allTiles.FindAll(x => x.Marks.Contains("freecpace")).Select(x => x.Position).ToList();
    }
    public void DeleteAllTiles()
    {
        ComletePlacing();
        while (allTiles.Count > 0)
            DeleteTile(allTiles.Last());
    }
    public Tile CreateTile(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        var tile = Instantiate(tilePrefab, transform).GetComponent<Tile>();
        allTiles.Add(tile);
        tile.Position = position;
        tile.Rotation = rotation;
        UpdateTilesSides(tile.Position);
        return tile;
    }

    #region Private methods
    void CreateFreeCpace(Vector2Int position)
    {
        _ = CreateTile(_freecpacePrefab, position, 0);
    }
    void ApplySelectedTile(Tile tile)
    {
        tile.transform.position = new Vector3(
            tile.transform.position.x,
            tile.transform.position.y + 3,
            tile.transform.position.z
            );
    }
    void CancelSelectedTile(Tile tile)
    {
        tile.transform.position = new Vector3(
            tile.transform.position.x,
            tile.transform.position.y - 3,
            tile.transform.position.z
            );
    }
    void DeleteTile(Tile tile)
    {
        Vector2Int clear_position = tile.Position;
        Destroy(tile.gameObject);
        allTiles.Remove(tile);
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
        var tile = allTiles.FirstOrDefault(x => x.Position == position);
        if (tile == null)
            return;
        List<Tile> tiles_around = new();
        for (int i = 1; i >= -1; i--)
        {
            for (int j = -1; j <= 1; j++)
            {
                var buffer = allTiles.FirstOrDefault(x => x.Position == new Vector2Int(tile.Position.x + j, tile.Position.y + i));
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
        var tile = allTiles.FirstOrDefault(x => x.Position == position);
        if (tile == null)
            return;
        List<Tile> tiles_around = new();
        for (int i = 1; i >= -1; i--)
        {
            for (int j = -1; j <= 1; j++)
            {
                var buffer = allTiles.FirstOrDefault(x => x.Position == new Vector2Int(tile.Position.x + j, tile.Position.y + i));
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
    #endregion
}
