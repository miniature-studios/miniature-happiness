using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileBuilder : MonoBehaviour
{
    [Header("==== Variables for debugging ====")]
    [SerializeField] public GameObject FreespacePrefab;
    [SerializeField] public GameObject StairsPrefab;
    [SerializeField] public GameObject WindowPrefab;
    [SerializeField] public GameObject OutdoorPrefab;
    [SerializeField] public GameObject CorridoorPrefab;
    [SerializeField] public GameObject WorkingPlaceFree;
    [SerializeField] public GameObject WorkingPlace;
    [SerializeField] public GameObject ChoosedTile;
    [Header("==== Scene composition Save/Loading ====")]
    [SerializeField] public GameObject LoadingPrefab;
    [SerializeField] public string SavingName = "SampleBuilding";

    [Header("==== Reqire variables ====")]
    [SerializeField] GameObject freespacePrefab;
    [SerializeField] GameObject rootObject;
    [SerializeField] BuilderMatrix builderMatrix;
    [SerializeField] public Gamemode GameMode;

    IValidator validator = new GameModeValidator();
    Dictionary<Vector2Int, TileUnion> tileUnionDictionary = new();
    TileUnion selectedTile = null;
    List<Vector2Int> previousPlaces;
    int previousRotation;
    bool justCreated = false;

    public TileUnion SelectedTile { get =>  selectedTile; }
    public GameObject RootObject { get => rootObject; set => rootObject = value; }
    public BuilderMatrix BuilderMatrix { get => builderMatrix; }
    public Dictionary<Vector2Int, TileUnion> TileUnionDictionary { get => tileUnionDictionary; }

    public void Start()
    {
        foreach (var union in rootObject.GetComponentsInChildren<TileUnion>())
        {
            if (!tileUnionDictionary.Values.Contains(union)) {
                foreach (var pos in union.TilesPositions)
                {
                    tileUnionDictionary.Add(pos, union);
                }
            }
        }
    }

    public Result Execute(ICommand command)
    {
        var response = validator.ValidateCommand(command);
        if (response.Success)
        {
            return command.Execute();
        }
        else
        {
            return response;
        }
    }
    public void ChangeGameMode(Gamemode gamemode)
    {
        validator = gamemode switch
        {
            Gamemode.godmode => new GodModeValidator(this),
            Gamemode.building => new BuildModeValidator(this),
            Gamemode.gameing => new GameModeValidator(),
            _ => throw new ArgumentException(),
        };
    }

    public void LoadSceneComposition(GameObject SceneCompositionPrefab)
    {
        DeleteAllTiles();
        if (rootObject != null)
        {
            DestroyImmediate(rootObject);
        }
        rootObject = Instantiate(SceneCompositionPrefab, transform);
        foreach (var union in rootObject.GetComponentsInChildren<TileUnion>())
        {
            foreach (var pos in union.TilesPositions)
            {
                tileUnionDictionary.Add(pos, union);
            }
        }
    }

    public Result SelectTile(TileUnion tile)
    {
        if (selectedTile == tile)
        {
            return new FailResult("Selected already selected tile");
        }
        else if(tile == null)
        {
            return new FailResult("Null");
        }
        else 
        {
            if (selectedTile != null) {
                var result = ComletePlacing();
                if (result.Failure)
                {
                    return result;
                }
            }
            selectedTile = tile;
            selectedTile.ApplySelecting();
            selectedTile.IsolateUpdate();
            previousPlaces = selectedTile.TilesPositions.ToList();
            previousRotation = selectedTile.Rotation;
            return new SuccessResult();
        }
    }
    public Result DeleteSelectedTile(out GameObject DeletedTileUI)
    {
        if (selectedTile == null)
        {
            DeletedTileUI = null;
            return new FailResult("Not selected Tile");
        }
        if (justCreated)
        {
            justCreated = false;
            DeletedTileUI = DeleteTile(selectedTile);
            selectedTile.CancelErrorTiles();
            selectedTile = null;
            return new FailResult("Selected tile deleted");
        }
        else
        {
            justCreated = false;
            DeletedTileUI = DeleteTile(selectedTile);
            foreach (var pos in previousPlaces)
            {
                tileUnionDictionary.Remove(pos);
            }
            foreach (var position in previousPlaces)
            {
                CreateTileAndBind(FreespacePrefab, position, 0);
            }
            UpdateSidesInPositions(previousPlaces);
            selectedTile.CancelErrorTiles();
            selectedTile = null;
            return new SuccessResult();
        }
    }
    public Result MoveSelectedTile(Direction direction)
    {
        if(selectedTile == null)
        {
            return new FailResult("Not selected Tile");
        }
        else
        {
            selectedTile.Move(direction);
            selectedTile.CancelErrorTiles();
            _ = selectedTile.TryApplyErrorTiles(this);
            return new SuccessResult();
        }
    }
    public Result RotateSelectedTile(Direction direction)
    {
        if (selectedTile == null)
        {
            return new FailResult("Not selected Tile");
        }
        else
        {
            selectedTile.Rotation += direction.GetIntRotationValue();
            selectedTile.CancelErrorTiles();
            _ = selectedTile.TryApplyErrorTiles(this);
            return new SuccessResult();
        }
    }
    public Result ComletePlacing()
    {
        if (selectedTile == null)
        {
            return new FailResult("Not selected Tile");
        }
        if(previousPlaces.Intersect(selectedTile.TilesPositions).Count() == previousPlaces.Count && previousRotation == selectedTile.Rotation && !justCreated)
        {
            UpdateSidesInPositions(selectedTile.TilesPositionsForUpdating);
            selectedTile.CancelSelecting();
            selectedTile.CancelErrorTiles();
            selectedTile = null;
            return new SuccessResult();
        }
        var tilesUnder = tileUnionDictionary.Where(x => selectedTile.TilesPositions.Contains(x.Key)).Select(x => x.Value).Distinct().ToList();
        tilesUnder.Remove(selectedTile);
        if (!tilesUnder.All(x => x.IsAllWithMark("freespace")))
        {
            return new FailResult("Not free spaces under");
        }
        if (selectedTile.TryApplyErrorTiles(this).Success)
        {
            return new FailResult("Cannot place tiles");
        }
        while (tilesUnder.Count > 0)
        {
            var buffer = tilesUnder.Last();
            tilesUnder.Remove(buffer);
            _ = DeleteTile(buffer);
        }
        if(!justCreated)
        {
            List<Vector2Int> bufferPositions = new();
            foreach (var position in previousPlaces)
            {
                if (tileUnionDictionary.ContainsKey(position))
                {
                    tileUnionDictionary.Remove(position);
                    if (!selectedTile.TilesPositions.Contains(position))
                    {
                        bufferPositions.Add(position);
                    }
                }
            }
            foreach (var pos in bufferPositions)
            {
                CreateTileAndBind(freespacePrefab, pos, 0);
            }
        }
        foreach (var pos in selectedTile.TilesPositions)
        {
            if (tileUnionDictionary.TryGetValue(pos, out TileUnion tileUnion))
            {
                RemoveTileFromDictionary(tileUnion);
            }
            tileUnionDictionary.Add(pos, selectedTile);
        }
        UpdateSidesInPositions(selectedTile.TilesPositionsForUpdating);
        if (!justCreated)
        {
            UpdateSidesInPositions(previousPlaces);
        }
        selectedTile.CancelSelecting();
        selectedTile.CancelErrorTiles();
        selectedTile = null;
        justCreated = false;
        return new SuccessResult();
    }
    public Result AddTileIntoBuilding(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        if (selectedTile != null)
        {
            return new FailResult("Complete placing previous tile");
        }
        justCreated = true;
        var tile = CreateTile(tilePrefab, position, rotation);
        return SelectTile(tile);
    }

    public void DeleteAllTiles()
    {
        while (tileUnionDictionary.Values.Count() > 0)
        {
            _ = DeleteTile(tileUnionDictionary.Values.Last());
        }
        TileUnionDictionary.Clear();
    }
    public void UpdateAllTiles()
    {
        foreach (var pair in TileUnionDictionary)
        {
            pair.Value.UpdateWallsInTile(this, pair.Key);
        }
        foreach (var pair in TileUnionDictionary)
        {
            pair.Value.UpdateCornersInTile(this, pair.Key);
        }
    }
    public IEnumerable<TileUnion> GetTileUnionsInPositions(IEnumerable<Vector2Int> positions)
    {
        return tileUnionDictionary.Where(x => positions.Contains(x.Key)).Select(x => x.Value).Distinct();
    }
    public IEnumerable<Vector2Int> GetInsideListPositions()
    {
        return tileUnionDictionary.Where(x => x.Value.IsAllWithMark("freespace")).Select(x => x.Key).OrderBy(x => Vector2Int.Distance(x, new(0, 0)));
    }
    public void CreateTileAndBind(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        TileUnion tileUnion = CreateTile(tilePrefab, position, rotation);
        foreach (var pos in tileUnion.TilesPositions)
        {
            TileUnionDictionary.Add(pos, tileUnion);
        }
        UpdateSidesInPositions(tileUnion.TilesPositionsForUpdating);
    }
    public TileUnion CreateTile(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        TileUnion tileUnion = Instantiate(tilePrefab, rootObject.transform).GetComponent<TileUnion>();
        tileUnion.Position = position;
        tileUnion.Rotation = rotation;
        return tileUnion;
    }

    GameObject DeleteTile(TileUnion tileUnion)
    {
        GameObject UIPrefab = tileUnion.UIPrefab;
        RemoveTileFromDictionary(tileUnion);
        DestroyImmediate(tileUnion.gameObject);
        return UIPrefab;
    }
    public void RemoveTileFromDictionary(TileUnion tileUnion)
    {
        foreach (var item in tileUnionDictionary.Where(x => x.Value == tileUnion).Distinct().ToList())
        {
            tileUnionDictionary.Remove(item.Key);
        }
    }
    void UpdateSidesInPositions(IEnumerable<Vector2Int> positions)
    {
        List<(TileUnion, Vector2Int)> queue = new();
        foreach (var position in positions)
        {
            if (TileUnionDictionary.TryGetValue(position, out var tile))
            {
                tile.UpdateWallsInTile(this, position);
                queue.Add((tile, position));
            }
        }
        foreach (var pair in queue)
        {
            pair.Item1.UpdateCornersInTile(this, pair.Item2);
        }
    }
}