using Common;
using System;
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
    [SerializeField] private GameObject freespacePrefab;
    [SerializeField] private GameObject rootObject;
    [SerializeField] private BuilderMatrix builderMatrix;
    [SerializeField] public Gamemode GameMode;
    private IValidator validator = new GameModeValidator();
    private List<Vector2Int> previousPlaces;
    private int previousRotation;
    private bool justCreated = false;

    public TileUnion SelectedTile { get; private set; } = null;
    public GameObject RootObject { get => rootObject; set => rootObject = value; }
    public BuilderMatrix BuilderMatrix => builderMatrix;
    public Dictionary<Vector2Int, TileUnion> TileUnionDictionary { get; } = new();

    public void Start()
    {
        foreach (TileUnion union in rootObject.GetComponentsInChildren<TileUnion>())
        {
            if (!TileUnionDictionary.Values.Contains(union))
            {
                foreach (Vector2Int pos in union.TilesPositions)
                {
                    TileUnionDictionary.Add(pos, union);
                }
            }
        }
        CreateNormalBuilding();
        UpdateAllTiles();
        ChangeGameMode(Gamemode.building);
    }

    public Result Execute(ICommand command)
    {
        Result response = validator.ValidateCommand(command);
        return response.Success ? command.Execute(this) : response;
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
        foreach (TileUnion union in rootObject.GetComponentsInChildren<TileUnion>())
        {
            foreach (Vector2Int pos in union.TilesPositions)
            {
                TileUnionDictionary.Add(pos, union);
            }
        }
        UpdateAllTiles();
    }

    public Result SelectTile(TileUnion tile)
    {
        if (SelectedTile == tile)
        {
            return new FailResult("Selected already selected tile");
        }
        else if (tile == null)
        {
            return new FailResult("Null");
        }
        else
        {
            if (SelectedTile != null)
            {
                Result result = ComletePlacing();
                if (result.Failure)
                {
                    return result;
                }
            }
            SelectedTile = tile;
            SelectedTile.ApplySelecting();
            SelectedTile.IsolateUpdate();
            previousPlaces = SelectedTile.TilesPositions.ToList();
            previousRotation = SelectedTile.Rotation;
            return new SuccessResult();
        }
    }
    public Result DeleteSelectedTile(out GameObject DeletedTileUI)
    {
        if (SelectedTile == null)
        {
            DeletedTileUI = null;
            return new FailResult("Not selected Tile");
        }
        if (justCreated)
        {
            justCreated = false;
            DeletedTileUI = DeleteTile(SelectedTile);
            SelectedTile = null;
            return new FailResult("Selected tile deleted");
        }
        else
        {
            justCreated = false;
            DeletedTileUI = DeleteTile(SelectedTile);
            foreach (Vector2Int pos in previousPlaces)
            {
                _ = TileUnionDictionary.Remove(pos);
            }
            foreach (Vector2Int position in previousPlaces)
            {
                CreateTileAndBind(FreespacePrefab, position, 0);
            }
            UpdateSidesInPositions(previousPlaces);
            SelectedTile = null;
            return new SuccessResult();
        }
    }
    public Result MoveSelectedTile(Direction direction)
    {
        if (SelectedTile == null)
        {
            return new FailResult("Not selected Tile");
        }
        else
        {
            SelectedTile.Move(direction);
            SelectedTile.ApplySelecting();
            _ = SelectedTile.TryApplyErrorTiles(this);
            return new SuccessResult();
        }
    }
    public Result RotateSelectedTile(Direction direction)
    {
        if (SelectedTile == null)
        {
            return new FailResult("Not selected Tile");
        }
        else
        {
            SelectedTile.SetRotation(SelectedTile.Rotation + direction.GetIntRotationValue());
            SelectedTile.ApplySelecting();
            _ = SelectedTile.TryApplyErrorTiles(this);
            return new SuccessResult();
        }
    }
    public Result ComletePlacing()
    {
        if (SelectedTile == null)
        {
            return new FailResult("Not selected Tile");
        }
        if (previousPlaces.Intersect(SelectedTile.TilesPositions).Count() == previousPlaces.Count && previousRotation == SelectedTile.Rotation && !justCreated)
        {
            UpdateSidesInPositions(SelectedTile.TilesPositionsForUpdating);
            SelectedTile.CancelSelecting();
            SelectedTile = null;
            return new SuccessResult();
        }
        List<TileUnion> tilesUnder = TileUnionDictionary.Where(x => SelectedTile.TilesPositions.Contains(x.Key)).Select(x => x.Value).Distinct().ToList();
        _ = tilesUnder.Remove(SelectedTile);
        if (!tilesUnder.All(x => x.IsAllWithMark("freespace")))
        {
            return new FailResult("Not free spaces under");
        }
        if (SelectedTile.TryApplyErrorTiles(this).Success)
        {
            return new FailResult("Cannot place tiles");
        }
        while (tilesUnder.Count > 0)
        {
            TileUnion buffer = tilesUnder.Last();
            _ = tilesUnder.Remove(buffer);
            _ = DeleteTile(buffer);
        }
        if (!justCreated)
        {
            List<Vector2Int> bufferPositions = new();
            foreach (Vector2Int position in previousPlaces)
            {
                if (TileUnionDictionary.ContainsKey(position))
                {
                    _ = TileUnionDictionary.Remove(position);
                    if (!SelectedTile.TilesPositions.Contains(position))
                    {
                        bufferPositions.Add(position);
                    }
                }
            }
            foreach (Vector2Int pos in bufferPositions)
            {
                CreateTileAndBind(freespacePrefab, pos, 0);
            }
        }
        foreach (Vector2Int pos in SelectedTile.TilesPositions)
        {
            if (TileUnionDictionary.TryGetValue(pos, out TileUnion tileUnion))
            {
                RemoveTileFromDictionary(tileUnion);
            }
            TileUnionDictionary.Add(pos, SelectedTile);
        }
        UpdateSidesInPositions(SelectedTile.TilesPositionsForUpdating);
        if (!justCreated)
        {
            UpdateSidesInPositions(previousPlaces);
        }
        SelectedTile.CancelSelecting();
        SelectedTile = null;
        justCreated = false;
        return new SuccessResult();
    }
    public Result AddTileIntoBuilding(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        if (SelectedTile != null)
        {
            return new FailResult("Complete placing previous tile");
        }
        justCreated = true;
        TileUnion tile = CreateTile(tilePrefab, position, rotation);
        return SelectTile(tile);
    }

    public void DeleteAllTiles()
    {
        while (TileUnionDictionary.Values.Count() > 0)
        {
            _ = DeleteTile(TileUnionDictionary.Values.Last());
        }
        TileUnionDictionary.Clear();
    }
    public void UpdateAllTiles()
    {
        foreach (KeyValuePair<Vector2Int, TileUnion> pair in TileUnionDictionary)
        {
            pair.Value.UpdateWalls(this, pair.Key);
        }
        foreach (KeyValuePair<Vector2Int, TileUnion> pair in TileUnionDictionary)
        {
            pair.Value.UpdateCorners(this, pair.Key);
        }
    }
    public IEnumerable<TileUnion> GetTileUnionsInPositions(IEnumerable<Vector2Int> positions)
    {
        return TileUnionDictionary.Where(x => positions.Contains(x.Key)).Select(x => x.Value).Distinct();
    }
    public IEnumerable<Vector2Int> GetInsideListPositions()
    {
        return TileUnionDictionary.Where(x => x.Value.IsAllWithMark("freespace")).Select(x => x.Key).OrderBy(x => Vector2Int.Distance(x, new(0, 0)));
    }
    public void CreateTileAndBind(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        TileUnion tileUnion = CreateTile(tilePrefab, position, rotation);
        foreach (Vector2Int pos in tileUnion.TilesPositions)
        {
            TileUnionDictionary.Add(pos, tileUnion);
        }
        UpdateSidesInPositions(tileUnion.TilesPositionsForUpdating);
    }
    public TileUnion CreateTile(GameObject tilePrefab, Vector2Int position, int rotation)
    {
        TileUnion tileUnion = Instantiate(tilePrefab, rootObject.transform).GetComponent<TileUnion>();
        tileUnion.SetPosition(position);
        tileUnion.SetRotation(rotation);
        return tileUnion;
    }

    private GameObject DeleteTile(TileUnion tileUnion)
    {
        GameObject UIPrefab = tileUnion.UIPrefab;
        DestroyImmediate(tileUnion.gameObject);
        RemoveTileFromDictionary(tileUnion);
        return UIPrefab;
    }
    public void RemoveTileFromDictionary(TileUnion tileUnion)
    {
        foreach (KeyValuePair<Vector2Int, TileUnion> item in TileUnionDictionary.Where(x => x.Value == tileUnion).Distinct().ToList())
        {
            _ = TileUnionDictionary.Remove(item.Key);
        }
    }

    private void UpdateSidesInPositions(IEnumerable<Vector2Int> positions)
    {
        List<(TileUnion, Vector2Int)> queue = new();
        foreach (Vector2Int position in positions)
        {
            if (TileUnionDictionary.TryGetValue(position, out TileUnion tile))
            {
                tile.UpdateWalls(this, position);
                queue.Add((tile, position));
            }
        }
        foreach ((TileUnion, Vector2Int) pair in queue)
        {
            pair.Item1.UpdateCorners(this, pair.Item2);
        }
    }

    // For test
    public void CreateNormalBuilding()
    {
        DeleteAllTiles();
        for (int i = 0; i < 9; i++)
        {
            CreateTileAndBind(OutdoorPrefab, new(0, i), 0);
        }

        for (int i = 0; i < 8; i++)
        {
            if (i == 1)
            {
                CreateTileAndBind(StairsPrefab, new(i + 1, 0), 0);
            }
            else
            {
                CreateTileAndBind(OutdoorPrefab, new(i + 1, 0), 0);
            }

            for (int j = 0; j < 7; j++)
            {
                if (j == 2)
                {
                    CreateTileAndBind(CorridoorPrefab, new(i + 1, j + 1), 0);
                }
                else if (j == 3)
                {
                    CreateTileAndBind(WorkingPlace, new(i + 1, j + 1), 0);
                }
                else if (j == 4)
                {
                    CreateTileAndBind(WorkingPlaceFree, new(i + 1, j + 1), 0);
                }
                else
                {
                    CreateTileAndBind(FreespacePrefab, new(i + 1, j + 1), 0);
                }
            }
            CreateTileAndBind(OutdoorPrefab, new(i + 1, 8), 0);
        }
        for (int i = 0; i < 9; i++)
        {
            CreateTileAndBind(OutdoorPrefab, new(9, i), 0);
        }
    }
}