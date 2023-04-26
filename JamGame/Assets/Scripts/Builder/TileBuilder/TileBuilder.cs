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
    [SerializeField] public GameObject BigWorkingPlace;
    [SerializeField] public Gamemode GameMode;
    [SerializeField] public string LoadPath = "/Saves/Random1.txt";
    [SerializeField] public string SavePath = "/Saves/Random1.txt";
    #endregion

    [SerializeField] GameObject pointerPrefab;
    [SerializeField] GameObject _freecpacePrefab;

    IValidator validator = new GameModeValidator();

    List<TileUnion> allTiles = new();

    // Buffer variables
    public TileUnion SelectedTile = null;
    GameObject pointer;

    Vector2Int previousPlace;
    List<Vector2Int> previousPlaces;
    int previousRotation;

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
        //TODO
        DeleteAllTiles();
        var infos = JsonConvert.DeserializeObject<List<TileInfo>>(File.ReadAllText(file_path));
        foreach (var tile in infos)
        {
            CreateTile(tile.prefab, tile.position, tile.rotation);
        }
    }
    public void SaveSceneComposition(string file_path)
    {
        // TODO
        File.WriteAllText(file_path, JsonConvert.SerializeObject(allTiles.Select(x => new TileInfo(x.gameObject, x.Position, x.Rotation)).ToList()));
    }

    public void SelectTile(Tile tile)
    {
        TileUnion picked = DetectTileUnion(tile);
        if (picked != null && SelectedTile != picked)
        {
            ComletePlacing();
            SelectedTile = picked;
            ApplySelectedTile(picked);
            previousPlace = SelectedTile.Position;
            previousPlaces = SelectedTile.TilesPositions;
            previousRotation = SelectedTile.Rotation;
            pointer = Instantiate(pointerPrefab, picked.TileUnionCenter, new Quaternion());
            IsolateUpdate(SelectedTile);
        }
    }
    public void DeleteSelectedTile()
    {
        var deletedPositions = new List<Vector2Int>(SelectedTile.TilesPositions);
        ComletePlacing();
        DeleteTile(SelectedTile);
        foreach (var position in deletedPositions)
        {
            CreateTile(FreecpacePrefab, position, 0);
        }
    }
    public void MoveSelectedTile(Direction direction)
    {
        SelectedTile.Move(direction);
        pointer.transform.position = SelectedTile.TileUnionCenter;
    }
    public void RotateSelectedTile()
    {
        SelectedTile.Rotation++;
        pointer.transform.position = SelectedTile.TileUnionCenter;
    }

    public void ComletePlacing()
    {
        if (SelectedTile == null)
            return;
        if(previousPlaces.Intersect(SelectedTile.TilesPositions).Count() == previousPlaces.Count && previousRotation == SelectedTile.Rotation)
        {
            UpdateSidesInPositions(SelectedTile.TilesPositionsForUpdating);
            CancelSelectedTile(SelectedTile);
            SelectedTile = null;
            Destroy(pointer.gameObject);
            return;
        }
        var tilesUnder = allTiles.FindAll(x => x != SelectedTile && x.TilesPositions.Intersect(SelectedTile.TilesPositions).ToList().Count > 0);
        if (validator is GodModeValidator)
        {
            if (tilesUnder == null)
            {
                UpdateSidesInPositions(SelectedTile.TilesPositionsForUpdating);
                UpdateSidesInPositions(previousPlaces);
                CancelSelectedTile(SelectedTile);
                SelectedTile = null;
                Destroy(pointer.gameObject);
                return;
            }
        }
        if (!tilesUnder.All(x => x.IsAllWithMark("freecpace")) || !IsValidPlacing(SelectedTile, SelectedTile.Position, SelectedTile.Rotation))
        {
            SelectedTile.Position = previousPlace;
            SelectedTile.Rotation = previousRotation;
        }
        else
        {
            while (tilesUnder.Count > 0)
            {
                var buffer = tilesUnder.Last();
                tilesUnder.Remove(buffer);
                DeleteTile(buffer);
            }
            foreach (var position in previousPlaces)
            {
                if(GetTileUnionsInPositions(new() { position }).Count == 0)
                    CreateFreeCpace(position);
            }
            UpdateSidesInPositions(SelectedTile.TilesPositionsForUpdating);
            UpdateSidesInPositions(previousPlaces);
        }
        CancelSelectedTile(SelectedTile);
        SelectedTile = null;
        Destroy(pointer.gameObject);
        
    }
    public void DeleteAllTiles()
    {
        ComletePlacing();
        while (allTiles.Count > 0)
            DeleteTile(allTiles.Last());
    }

    public bool IsValidPlacing(TileUnion tileUnion, Vector2Int position, int rotation)
    {
        var ImPlace = tileUnion.GetImagineUpdatingPlaces(position, rotation);
        foreach (var pos in ImPlace.Item2)
        {
            var unions = GetTileUnionsInPositions(new() { pos });
            if (unions.Count == 0)
                ImPlace.Item1.Add(pos, null);
            else if (unions.Count == 1)
                ImPlace.Item1.Add(pos, unions.First().GetTile(pos));
            else
                throw new Exception();
        }
        return tileUnion.IsValidPlacing(ImPlace.Item1, position, rotation).Count == 0;
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
    public TileUnion AddTileIntoBuilding(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        var places = tilePrefab.GetComponent<TileUnion>().GetImaginePlaces(position, rotation);
        List<TileUnion> toDelete = new();
        foreach (var tile in allTiles)
        {
            if (tile.TilesPositions.Intersect(places).Count() > 0)
            {
                toDelete.Add(tile);
            }
        }
        foreach (var tile in toDelete)
        {
            DeleteTile(tile);
        }
        return CreateTile(tilePrefab, position, rotation);
    }
    public TileUnion CreateTile(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        var tileUnion = Instantiate(tilePrefab, transform).GetComponent<TileUnion>();
        allTiles.Add(tileUnion);
        tileUnion.Position = position;
        tileUnion.Rotation = rotation;
        UpdateSidesInPositions(tileUnion.TilesPositionsForUpdating);
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
    void DeleteTile(TileUnion tileUnion)
    {
        Destroy(tileUnion.gameObject);
        allTiles.Remove(tileUnion);
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