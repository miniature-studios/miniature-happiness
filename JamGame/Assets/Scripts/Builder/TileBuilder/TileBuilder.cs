using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileBuilder : MonoBehaviour
{
    #region For Inspector
    [SerializeField] public GameObject FreespacePrefab;
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
    [SerializeField] GameObject freespacePrefab;
    [SerializeField] GameObject errorPlacingPrefab;
    [SerializeField] GameObject rootObject;
    [SerializeField] WallSolver wallSolver;

    IValidator validator = new GameModeValidator();

    List<TileUnion> allTiles = new();
    Dictionary<Vector2Int, Tile> tilesDictionary = new();

    // Buffer variables
    TileUnion selectedTile = null;
    GameObject pointer;
    List<GameObject> errors = new();

    List<Vector2Int> previousPlaces;
    int previousRotation;
    bool justCreated = false;

    public TileUnion SelectedTile { get { return selectedTile; } }
    public GameObject RootObject
    {
        get
        {
            if(rootObject == null)
                rootObject = Instantiate(new GameObject("RootObject"), transform);
            return rootObject;
        }
        set
        {
            rootObject = value;
        }
    }
    public WallSolver WallSolver { get { return wallSolver; } }
    public Dictionary<Vector2Int, Tile> TilesDictionary { get { return tilesDictionary; } }

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
        if(RootObject != null) DestroyImmediate(RootObject);
        RootObject = Instantiate(SceneCompositionPrefab, transform);
        foreach (var union in rootObject.GetComponentsInChildren<TileUnion>())
        {
            allTiles.Add(union);
        }
    }

    public Response SelectTile(Tile tile)
    {
        TileUnion picked = DetectTileUnion(tile);
        if (selectedTile == picked)
        {
            return new Response("Selected already selected tile", false);
        }
        else if(picked == null)
        {
            return new Response("Null", false);
        }
        else 
        {
            if (selectedTile != null) {
                var response = ComletePlacing();
                if (!response.Accepted)
                {
                    return response;
                }
            }
            selectedTile = picked;
            previousPlaces = selectedTile.TilesPositions.ToList();
            previousRotation = selectedTile.Rotation;
            ApplySelectedTile(picked);
            pointer = Instantiate(pointerPrefab, picked.TileUnionCenter, new Quaternion(), rootObject.transform);
            selectedTile.IsolateUpdate();
            return new Response("Selected", true);
        }
    }
    public Response DeleteSelectedTile(out GameObject DeletedTileUI)
    {
        if (selectedTile == null)
        {
            DeletedTileUI = null;
            return new Response("Not selected Tile", false);
        }
        if (justCreated)
        {
            justCreated = false;
            DeletedTileUI = DeleteTile(selectedTile);
            selectedTile = null;
            DestroyImmediate(pointer.gameObject);
            CancelErrorPlacing();
            return new Response("Selected tile deleted", true);
        }
        else
        {
            justCreated = false;
            DeletedTileUI = DeleteTile(selectedTile);
            foreach (var pos in previousPlaces)
            {
                tilesDictionary.Remove(pos);
            }
            foreach (var position in previousPlaces)
            {
                _ = CreateTile(FreespacePrefab, position, 0);
            }
            UpdateSidesInPositions(previousPlaces);
            DestroyImmediate(pointer.gameObject);
            CancelErrorPlacing();
            selectedTile = null;
            return new Response("Selected tile deleted", true);
        }
    }
    public Response MoveSelectedTile(Direction direction)
    {
        if(selectedTile == null)
        {
            return new Response("Not selected Tile", false);
        }
        else
        {
            selectedTile.Move(direction);
            pointer.transform.position = selectedTile.TileUnionCenter;
            CancelErrorPlacing();
            ApplyErrorPlacing(selectedTile);
            return new Response("Selected tile moved", true);
        }
    }
    public Response RotateSelectedTile(Direction direction)
    {
        if (selectedTile == null)
        {
            return new Response("Not selected Tile", false);
        }
        else
        {
            selectedTile.Rotation += direction.GetIntRotationValue();
            pointer.transform.position = selectedTile.TileUnionCenter;
            CancelErrorPlacing();
            ApplyErrorPlacing(selectedTile);
            return new Response("Selected tile rotated", true);
        }
    }
    public Response ComletePlacing()
    {
        if (selectedTile == null)
        {
            return new Response("Not selected Tile", false);
        }
        if(previousPlaces.Intersect(selectedTile.TilesPositions).Count() == previousPlaces.Count && previousRotation == selectedTile.Rotation && !justCreated)
        {
            CancelSelectedTile(selectedTile);
            selectedTile = null;
            DestroyImmediate(pointer.gameObject);
            CancelErrorPlacing();
            return new Response("same place", true);
        }
        var tilesUnder = allTiles.FindAll(x => x != selectedTile && x.TilesPositions.Intersect(selectedTile.TilesPositions).ToList().Count > 0);
        if (!tilesUnder.All(x => x.IsAllWithMark("freespace")))
        {
            return new Response("Not free spaces under", false);
        }
        List<Tile> incorrectTiles = new();
        if (!IsValidPlacing(selectedTile, out incorrectTiles))
        {
            return new Response($"Cannot place {incorrectTiles.Count} tiles", false);
        }
        // All good
        while (tilesUnder.Count > 0)
        {
            var buffer = tilesUnder.Last();
            foreach (var pos in buffer.TilesPositions)
            {
                tilesDictionary.Remove(pos);
            }
            tilesUnder.Remove(buffer);
            _ = DeleteTile(buffer);
        }
        if(!justCreated)
        {
            foreach (var position in previousPlaces)
            {
                if (GetTileUnionsInPositions(new List<Vector2Int>{ position }).Count() == 0)
                {
                    tilesDictionary.Remove(position);
                    CreateFreeSpace(position);
                }
                else
                {
                    tilesDictionary.Remove(position);
                }
            }
        }
        foreach (var pos in selectedTile.TilesPositions)
        {
            tilesDictionary.Remove(pos);
            tilesDictionary.Add(pos, selectedTile.GetTile(pos));
        }
        UpdateSidesInPositions(selectedTile.TilesPositionsForUpdating);
        if (!justCreated)
        {
            UpdateSidesInPositions(previousPlaces);
        }
        CancelSelectedTile(selectedTile);
        selectedTile = null;
        DestroyImmediate(pointer.gameObject);
        CancelErrorPlacing();
        justCreated = false;
        return new Response("Placed", true);
    }
    public Response AddTileIntoBuilding(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        if (selectedTile != null)
            return new Response("Complete placing previous tile", false);
        justCreated = true;
        var tile = CreateTile(tilePrefab, position, rotation, false);
        var response = SelectTile(tile.GetTile(tile.TilesPositions.First()));
        if (response.Accepted)
        {
            return new Response("Tile Added", true);
        }
        else
        {
            return response;
        }
    }

    public void DeleteAllTiles()
    {
        while (allTiles.Count > 0)
            _ = DeleteTile(allTiles.Last());
        TilesDictionary.Clear();
    }
    public void UpdateAllTiles()
    {
        foreach (var pair in TilesDictionary)
        {
            pair.Value.UpdateWalls(this, pair.Key).UpdateWalls();
        }
    }
    public bool IsValidPlacing(TileUnion tileUnion, out List<Tile> incorrectTiles)
    {
        incorrectTiles = tileUnion.GetOverlappingWalls(this);
        return incorrectTiles.Count == 0;
    }
    public IEnumerable<TileUnion> GetTileUnionsInPositions(IEnumerable<Vector2Int> positions)
    {
        foreach (var tile in allTiles)
        {
            if(tile.TilesPositions.Intersect(positions).Count() > 0)
                yield return tile;
        }
    }
    public IEnumerable<Vector2Int> GetInsideListPositions()
    {
        return allTiles
            .FindAll(x => x.IsAllWithMark("freespace"))
            .Select(x => x.TilesPositions)
            .Aggregate((x,y) => x.Concat(y).ToList())
            .OrderBy(x => Vector2Int.Distance(x, new(0,0)));
    }
    public TileUnion DetectTileUnion(Tile tile)
    {
        return allTiles.Find(x => x.IsContainsTile(tile));
    }
    public TileUnion CreateTile(GameObject tilePrefab, Vector2Int position, int rotation, bool update = true)
    {
        TileUnion tileUnion = Instantiate(tilePrefab, RootObject.transform).GetComponent<TileUnion>();
        allTiles.Add(tileUnion);
        tileUnion.Position = position;
        tileUnion.Rotation = rotation;
        if (update)
        {
            foreach (var pos in tileUnion.TilesPositions)
            {
                TilesDictionary.Add(pos, tileUnion.GetTile(pos));
            }
            UpdateSidesInPositions(tileUnion.TilesPositionsForUpdating);
        }
        return tileUnion;
    }

    #region Private methods
    void CreateFreeSpace(Vector2Int position)
    {
        _ = CreateTile(freespacePrefab, position, 0);
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
    GameObject DeleteTile(TileUnion tileUnion)
    {
        GameObject UIPrefab = tileUnion.UIPrefab;
        DestroyImmediate(tileUnion.gameObject);
        allTiles.Remove(tileUnion);
        return UIPrefab;
    }
    void UpdateSidesInPositions(IEnumerable<Vector2Int> positions)
    {
        List<(Tile, Vector2Int)> queue = new();
        foreach (var position in positions)
        {
            if (TilesDictionary.TryGetValue(position, out var tile))
            {
                tile.UpdateWalls(this, position).UpdateWalls();
                queue.Add((tile, position));
            }
        }
        foreach (var pair in queue)
        {
            pair.Item1.UpdateCorners(this, pair.Item2);
        }
    }
    #endregion
}