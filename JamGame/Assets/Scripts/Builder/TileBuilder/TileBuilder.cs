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
    [SerializeField] public GameObject ChoosedTile;
    [SerializeField] public Gamemode GameMode;
    [SerializeField] public string LoadPath = "/Saves/Random1.txt";
    [SerializeField] public string SavePath = "/Saves/Random1.txt";
    #endregion

    [SerializeField] GameObject pointerPrefab;
    [SerializeField] GameObject _freecpacePrefab;
    [SerializeField] GameObject errorPlacingPrefab;

    IValidator validator = new GameModeValidator();

    List<TileUnion> allTiles = new();

    // Buffer variables
    public TileUnion SelectedTile = null;
    GameObject pointer;
    List<GameObject> errors = new();

    bool justCreated = false;
    Vector2Int previousPlace;
    List<Vector2Int> previousPlaces;
    int previousRotation;

    public Answer Execute(ICommand command)
    {
        var ansver = validator.ValidateCommand(command);
        if (ansver.Accepted)
            return command.Execute();
        else
            return ansver;
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
        //TODO
        DeleteAllTiles();
        var infos = JsonConvert.DeserializeObject<List<TileInfo>>(File.ReadAllText(file_path));
        foreach (var tile in infos)
        {
            _ = CreateTile(tile.prefab, tile.position, tile.rotation);
        }
    }
    public void SaveSceneComposition(string file_path)
    {
        // TODO
        File.WriteAllText(file_path, JsonConvert.SerializeObject(allTiles.Select(x => new TileInfo(x.gameObject, x.Position, x.Rotation)).ToList()));
    }

    public Answer SelectTile(Tile tile)
    {
        TileUnion picked = DetectTileUnion(tile);
        if (SelectedTile == picked)
        {
            return new Answer("Selected already selected tile", false);
        }
        else if(picked == null)
        {
            return new Answer("Null", false);
        }
        else 
        {
            if (SelectedTile != null) {
                var ansver = ComletePlacing();
                if (!ansver.Accepted)
                    return ansver;
            }
            SelectedTile = picked;
            ApplySelectedTile(picked);
            previousPlace = SelectedTile.Position;
            previousPlaces = SelectedTile.TilesPositions;
            previousRotation = SelectedTile.Rotation;
            pointer = Instantiate(pointerPrefab, picked.TileUnionCenter, new Quaternion());
            IsolateUpdate(SelectedTile);
            ApplyErrorPlacing(SelectedTile);
            return new Answer("Selected", true);
        }
    }
    public Answer DeleteSelectedTile(out GameObject destroyedTile)
    {
        if (SelectedTile == null)
        {
            destroyedTile = null;
            return new Answer("Not selected Tile", false);
        }
        if (justCreated)
        {
            justCreated = false;
            destroyedTile = DeleteTile(SelectedTile);
            Destroy(pointer.gameObject);
            CancelErrorPlacing();
            return new Answer("Selected tile deleted", true);
        }
        else
        {
            var deletedPositions = new List<Vector2Int>(SelectedTile.TilesPositions);
            destroyedTile = DeleteTile(SelectedTile);
            foreach (var position in deletedPositions)
            {
                _ = CreateTile(FreecpacePrefab, position, 0);
            }
            UpdateSidesInPositions(deletedPositions);
            justCreated = false;
            Destroy(pointer.gameObject);
            CancelErrorPlacing();
            return new Answer("Selected tile deleted", true);
        }
    }
    public Answer MoveSelectedTile(Direction direction)
    {
        if(SelectedTile == null)
        {
            return new Answer("Not selected Tile", false);
        }
        else
        {
            SelectedTile.Move(direction);
            pointer.transform.position = SelectedTile.TileUnionCenter;
            CancelErrorPlacing();
            ApplyErrorPlacing(SelectedTile);
            return new Answer("Selected tile moved", true);
        }
    }
    public Answer RotateSelectedTile()
    {
        if (SelectedTile == null)
        {
            return new Answer("Not selected Tile", false);
        }
        else
        {
            SelectedTile.Rotation++;
            pointer.transform.position = SelectedTile.TileUnionCenter;
            CancelErrorPlacing();
            ApplyErrorPlacing(SelectedTile);
            return new Answer("Selected tile rotated", true);
        }
    }

    public Answer ComletePlacing()
    {
        if (SelectedTile == null)
        {
            return new Answer("Not selected Tile", false);
        }
        if(previousPlaces.Intersect(SelectedTile.TilesPositions).Count() == previousPlaces.Count && previousRotation == SelectedTile.Rotation && !justCreated)
        {
            UpdateSidesInPositions(SelectedTile.TilesPositionsForUpdating);
            CancelSelectedTile(SelectedTile);
            SelectedTile = null;
            Destroy(pointer.gameObject);
            CancelErrorPlacing();
            return new Answer("same place", true);
        }
        var tilesUnder = allTiles.FindAll(x => x != SelectedTile && x.TilesPositions.Intersect(SelectedTile.TilesPositions).ToList().Count > 0);
        if (validator is GodModeValidator && tilesUnder == null)
        {
            UpdateSidesInPositions(SelectedTile.TilesPositionsForUpdating);
            UpdateSidesInPositions(previousPlaces);
            CancelSelectedTile(SelectedTile);
            SelectedTile = null;
            Destroy(pointer.gameObject);
            justCreated = false;
            CancelErrorPlacing();
            return new Answer("Placed in the void |GodMode|", true);
        }

        if (!tilesUnder.All(x => x.IsAllWithMark("freecpace")))
        {
            return new Answer("Not free spaces under", false);
        }
        List<Tile> incorrectTiles = new();
        if (!IsValidPlacing(SelectedTile, out incorrectTiles))
        {
            return new Answer($"Cannot place {incorrectTiles.Count} tiles", false);
        }

        while (tilesUnder.Count > 0)
        {
            var buffer = tilesUnder.Last();
            tilesUnder.Remove(buffer);
            _ = DeleteTile(buffer);
        }
        foreach (var position in previousPlaces)
        {
            if(GetTileUnionsInPositions(new() { position }).Count == 0)
                CreateFreeCpace(position);
        }
        UpdateSidesInPositions(SelectedTile.TilesPositionsForUpdating);
        UpdateSidesInPositions(previousPlaces);
        CancelSelectedTile(SelectedTile);
        SelectedTile = null;
        Destroy(pointer.gameObject);
        justCreated = false;
        CancelErrorPlacing();
        return new Answer("Placed", true);
    }
    public Answer DeleteAllTiles()
    {
        while (allTiles.Count > 0)
            _ = DeleteTile(allTiles.Last());
        return new Answer("Deleted all tiles", true);
    }

    public Answer AddTileIntoBuilding(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        if(justCreated)
            return new Answer("Complete placing previous tile", false);
        var tile = CreateTile(tilePrefab, position, rotation, false);
        var ansver = SelectTile(tile.GetTile(tile.TilesPositions.First()));
        if (ansver.Accepted)
        {
            justCreated = true;
            return new Answer("Tile Added", true);
        }
        else
            return ansver;
    }

    public bool IsValidPlacing(TileUnion tileUnion, out List<Tile> incorrectTiles)
    {
        Dictionary<Vector2Int, Tile> tilePairs = new();
        var needed = tileUnion.TilesPositionsForUpdating;
        foreach (var position in needed)
        {
            if (tileUnion.TilesPositions.Contains(position))
            {
                tilePairs.Add(position, tileUnion.GetTile(position));
                continue;
            }
            foreach (var tile in allTiles)
            {
                if (tile.TilesPositions.Contains(position))
                {
                    tilePairs.Add(position, tile.GetTile(position));
                    break;
                }
            }
            if (!tilePairs.ContainsKey(position))
                tilePairs.Add(position, null);
        }
        incorrectTiles = tileUnion.IsValidPlacing(tilePairs);
        return incorrectTiles.Count == 0;
    }
    public List<TileUnion> GetTileUnionsInPositions(List<Vector2Int> positions)
    {
        List<TileUnion> result = new();
        foreach (var tile in allTiles)
        {
            if(tile.TilesPositions.Intersect(positions).Count() > 0)
                result.Add(tile);
        }
        return result;
    }
    public List<Vector2Int> GetInsideListPositions()
    {
        return allTiles.FindAll(x => x.IsAllWithMark("freecpace")).Select(x => x.TilesPositions).Aggregate((x,y) => x.Concat(y).ToList()).ToList();
    }
    public TileUnion DetectTileUnion(Tile tile)
    {
        return allTiles.Find(x => x.IsContainsTile(tile));
    }
    public TileUnion CreateTile(GameObject tilePrefab, Vector2Int position, int rotation, bool update = true)
    {
        var tileUnion = Instantiate(tilePrefab, transform).GetComponent<TileUnion>();
        allTiles.Add(tileUnion);
        tileUnion.Position = position;
        tileUnion.Rotation = rotation;
        if(update) UpdateSidesInPositions(tileUnion.TilesPositionsForUpdating);
        return tileUnion;
    }

    #region Private methods
    void CreateFreeCpace(Vector2Int position)
    {
        _ = CreateTile(_freecpacePrefab, position, 0);
    }
    void ApplySelectedTile(TileUnion tileUnion)
    {
        tileUnion.transform.position = new Vector3(
            tileUnion.transform.position.x,
            tileUnion.transform.position.y + 3,
            tileUnion.transform.position.z
            );
    }
    void CancelSelectedTile(TileUnion tileUnion)
    {
        tileUnion.transform.position = new Vector3(
            tileUnion.transform.position.x,
            tileUnion.transform.position.y - 3,
            tileUnion.transform.position.z
            );
    }
    void ApplyErrorPlacing(TileUnion tileUnion)
    {
        _ = IsValidPlacing(tileUnion, out List<Tile> errorTiles);
        foreach (var tile in errorTiles)
        {
            errors.Add(Instantiate(errorPlacingPrefab, tile.transform.position, new(), transform));
        }
    }
    void CancelErrorPlacing()
    {
        while(errors.Count > 0)
        {
            Destroy(errors.Last().gameObject);
            errors.Remove(errors.Last());
        }
    }
    GameObject DeleteTile(TileUnion tileUnion)
    {
        Destroy(tileUnion.gameObject);
        allTiles.Remove(tileUnion);
        return null; // TODO
    }
    void UpdateSidesInPositions(List<Vector2Int> positions)
    {
        foreach (var tile in allTiles)
        {
            if (tile.TilesPositions.Intersect(positions).Count() > 0)
            {
                UpdateWallsInTileUnion(tile);
            }
        }
        foreach (var tile in allTiles)
        {
            if (tile.TilesPositions.Intersect(positions).Count() > 0)
            {
                UpdateCornersInTileUnion(tile);
            }
        }
    }
    void UpdateWallsInTileUnion(TileUnion tileUnion)
    {
        Dictionary<Vector2Int, Tile> tilePairs = new();
        var needed = tileUnion.TilesPositionsForUpdating;
        foreach (var position in needed)
        {
            if (tileUnion.TilesPositions.Contains(position))
            {
                tilePairs.Add(position, tileUnion.GetTile(position));
                continue;
            }
            foreach (var tile in allTiles)
            {
                if(tile.TilesPositions.Contains(position))
                {
                    tilePairs.Add(position, tile.GetTile(position));
                    break;
                }
            }
            if(!tilePairs.ContainsKey(position))
                tilePairs.Add(position, null);
        }
        tileUnion.UpdateWalls(tilePairs);
    }
    void UpdateCornersInTileUnion(TileUnion tileUnion)
    {
        Dictionary<Vector2Int, Tile> tilePairs = new();
        var needed = tileUnion.TilesPositionsForUpdating;
        foreach (var position in needed)
        {
            if (tileUnion.TilesPositions.Contains(position))
            {
                tilePairs.Add(position, tileUnion.GetTile(position));
                continue;
            }   
            foreach (var tile in allTiles)
            {
                if (tile.TilesPositions.Contains(position))
                {
                    tilePairs.Add(position, tile.GetTile(position));
                }
            }
            if (!tilePairs.ContainsKey(position))
                tilePairs.Add(position, null);
        }
        tileUnion.UpdateCorners(tilePairs);
    }
    public void IsolateUpdate(TileUnion tileUnion)
    {
        Dictionary<Vector2Int, Tile> tilePairs = new();
        var needed = tileUnion.TilesPositionsForUpdating;
        foreach (var position in needed)
        {
            if(tileUnion.TilesPositions.Contains(position))
                tilePairs.Add(position, tileUnion.GetTile(position));
            else
                tilePairs.Add(position, null);
        }
        tileUnion.UpdateWalls(tilePairs);
        tileUnion.UpdateCorners(tilePairs);
    }
    #endregion
}

public class Answer
{
    string massage;
    bool accepted;
    public Answer(string massage, bool accepted)
    {
        this.massage = massage;
        this.accepted = accepted;
    }
    public string Massage
    {
        get { return massage; }
    }
    public bool Accepted
    {
        get { return accepted; }
    }
}