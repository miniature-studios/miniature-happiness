using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    [SerializeField] public GameObject SceneCompositionLoadingPrefab;
    [SerializeField] public string SceneCompositionPrefabSavingName = "SampleBuilding";
    #endregion

    [SerializeField] GameObject pointerPrefab;
    [SerializeField] GameObject _freecpacePrefab;
    [SerializeField] GameObject errorPlacingPrefab;
    [SerializeField] public GameObject rootObject;

    IValidator validator = new GameModeValidator();

    [SerializeField] List<TileUnion> allTiles = new();

    // Buffer variables
    public TileUnion SelectedTile = null;
    GameObject pointer;
    List<GameObject> errors = new();

    bool justCreated = false;
    List<Vector2Int> previousPlaces;
    int previousRotation;

    public void Awake()
    {
        if(rootObject == null)
        {
            rootObject = Instantiate(new GameObject("RootObject"), transform);
        }
    }

    public Response Execute(ICommand command)
    {
        var response = validator.ValidateCommand(command);
        if (response.Accepted)
            return command.Execute();
        else
            return response;
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

    public void LoadSceneComposition(GameObject SceneCompositionPrefab)
    {
        DeleteAllTiles();
        if(rootObject != null) DestroyImmediate(rootObject);
        rootObject = Instantiate(SceneCompositionPrefab, transform);
        foreach (var union in rootObject.GetComponentsInChildren<TileUnion>())
        {
            allTiles.Add(union);
        }
    }

    public Response SelectTile(Tile tile)
    {
        TileUnion picked = DetectTileUnion(tile);
        if (SelectedTile == picked)
        {
            return new Response("Selected already selected tile", false);
        }
        else if(picked == null)
        {
            return new Response("Null", false);
        }
        else 
        {
            if (SelectedTile != null) {
                var response = ComletePlacing();
                if (!response.Accepted)
                    return response;
            }
            SelectedTile = picked;
            ApplySelectedTile(picked);
            previousPlaces = SelectedTile.TilesPositions;
            previousRotation = SelectedTile.Rotation;
            pointer = Instantiate(pointerPrefab, picked.TileUnionCenter, new Quaternion(), rootObject.transform);
            IsolateUpdate(SelectedTile);
            ApplyErrorPlacing(SelectedTile);
            return new Response("Selected", true);
        }
    }
    public Response DeleteSelectedTile(out GameObject destroyedTile)
    {
        if (SelectedTile == null)
        {
            destroyedTile = null;
            return new Response("Not selected Tile", false);
        }
        if (justCreated)
        {
            justCreated = false;
            destroyedTile = DeleteTile(SelectedTile, false);
            SelectedTile = null;
            DestroyImmediate(pointer.gameObject);
            CancelErrorPlacing();
            return new Response("Selected tile deleted", true);
        }
        else
        {
            var deletedPositions = new List<Vector2Int>(previousPlaces);
            destroyedTile = DeleteTile(SelectedTile, false);
            SelectedTile = null;
            foreach (var position in deletedPositions)
            {
                _ = CreateTile(FreecpacePrefab, position, 0);
            }
            UpdateSidesInPositions(deletedPositions);
            justCreated = false;
            DestroyImmediate(pointer.gameObject);
            CancelErrorPlacing();
            return new Response("Selected tile deleted", true);
        }
    }
    public Response MoveSelectedTile(Direction direction)
    {
        if(SelectedTile == null)
        {
            return new Response("Not selected Tile", false);
        }
        else
        {
            SelectedTile.Move(direction);
            pointer.transform.position = SelectedTile.TileUnionCenter;
            CancelErrorPlacing();
            ApplyErrorPlacing(SelectedTile);
            return new Response("Selected tile moved", true);
        }
    }
    public Response RotateSelectedTile()
    {
        if (SelectedTile == null)
        {
            return new Response("Not selected Tile", false);
        }
        else
        {
            SelectedTile.Rotation++;
            pointer.transform.position = SelectedTile.TileUnionCenter;
            CancelErrorPlacing();
            ApplyErrorPlacing(SelectedTile);
            return new Response("Selected tile rotated", true);
        }
    }
    public Response ComletePlacing()
    {
        if (SelectedTile == null)
        {
            return new Response("Not selected Tile", false);
        }
        if(previousPlaces.Intersect(SelectedTile.TilesPositions).Count() == previousPlaces.Count && previousRotation == SelectedTile.Rotation && !justCreated)
        {
            UpdateSidesInPositions(SelectedTile.TilesPositionsForUpdating);
            CancelSelectedTile(SelectedTile);
            SelectedTile = null;
            DestroyImmediate(pointer.gameObject);
            CancelErrorPlacing();
            return new Response("same place", true);
        }
        var tilesUnder = allTiles.FindAll(x => x != SelectedTile && x.TilesPositions.Intersect(SelectedTile.TilesPositions).ToList().Count > 0);
        if (validator is GodModeValidator && tilesUnder == null)
        {
            UpdateSidesInPositions(SelectedTile.TilesPositionsForUpdating);
            UpdateSidesInPositions(previousPlaces);
            CancelSelectedTile(SelectedTile);
            SelectedTile = null;
            DestroyImmediate(pointer.gameObject);
            justCreated = false;
            CancelErrorPlacing();
            return new Response("Placed in the void |GodMode|", true);
        }

        if (!tilesUnder.All(x => x.IsAllWithMark("freecpace")))
        {
            return new Response("Not free spaces under", false);
        }
        List<Tile> incorrectTiles = new();
        if (!IsValidPlacing(SelectedTile, out incorrectTiles))
        {
            return new Response($"Cannot place {incorrectTiles.Count} tiles", false);
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
        DestroyImmediate(pointer.gameObject);
        justCreated = false;
        CancelErrorPlacing();
        return new Response("Placed", true);
    }
    public Response AddTileIntoBuilding(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        if (justCreated || SelectedTile != null)
            return new Response("Complete placing previous tile", false);
        var tile = CreateTile(tilePrefab, position, rotation, false);
        var response = SelectTile(tile.GetTile(tile.TilesPositions.First()));
        if (response.Accepted)
        {
            justCreated = true;
            return new Response("Tile Added", true);
        }
        else
            return response;
    }

    public void DeleteAllTiles()
    {
        while (allTiles.Count > 0)
            _ = DeleteTile(allTiles.Last());
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
        return allTiles
            .FindAll(x => x.IsAllWithMark("freecpace"))
            .Select(x => x.TilesPositions)
            .Aggregate((x,y) => x.Concat(y).ToList())
            .OrderBy(x => Vector2Int.Distance(x, new(0,0)))
            .ToList();
    }
    public TileUnion DetectTileUnion(Tile tile)
    {
        return allTiles.Find(x => x.IsContainsTile(tile));
    }
    public TileUnion CreateTile(GameObject tilePrefab, Vector2Int position, int rotation, bool update = true)
    {
        TileUnion tileUnion;
        if (tilePrefab.scene.IsValid()) {
            tilePrefab.gameObject.SetActive(true);
            tileUnion = tilePrefab.GetComponent<TileUnion>();
        }
        else {
            tileUnion = Instantiate(tilePrefab, rootObject.transform).GetComponent<TileUnion>();
        }
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
            3,
            tileUnion.transform.position.z
            );
    }
    void CancelSelectedTile(TileUnion tileUnion)
    {
        tileUnion.transform.position = new Vector3(
            tileUnion.transform.position.x,
            0,
            tileUnion.transform.position.z
            );
    }
    void ApplyErrorPlacing(TileUnion tileUnion)
    {
        _ = IsValidPlacing(tileUnion, out List<Tile> errorTiles);
        foreach (var tile in errorTiles)
        {
            errors.Add(Instantiate(errorPlacingPrefab, tile.transform.position, new(), rootObject.transform));
        }
    }
    void CancelErrorPlacing()
    {
        while(errors.Count > 0)
        {
            DestroyImmediate(errors.Last().gameObject);
            errors.Remove(errors.Last());
        }
    }
    GameObject DeleteTile(TileUnion tileUnion, bool forever = true)
    {
        var DeletedGameObject = tileUnion.UnionPrefab;
        if (forever)
        {
            DestroyImmediate(tileUnion.gameObject);
            allTiles.Remove(tileUnion);
            return DeletedGameObject;
        }
        else
        {
            tileUnion.gameObject.SetActive(false);
            allTiles.Remove(tileUnion);
            return tileUnion.gameObject;
        }
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