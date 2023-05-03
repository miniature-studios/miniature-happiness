using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WallPrefabHandler
{
    public TileWallType Type;
    public GameObject Prefab;
}
[Serializable]
public class WallCollection
{
    public Direction Place;
    public List<WallPrefabHandler> Handlers;
    public void SetWall(TileWallType type)
    {
        foreach (var handler in Handlers)
        {
            if(handler.Type == type)
                handler.Prefab.SetActive(true);
            else
                handler.Prefab.SetActive(false);
        }
    }
    public bool IsActive(TileWallType tileWallType)
    {
        if(Handlers.Find(x => x.Type == tileWallType) == null)
            return false;
        return Handlers.Find(x => x.Type == tileWallType).Prefab.gameObject.activeSelf;
    }
}

[Serializable]
public class CornerPrefabHandler
{
    public TileCornerType Type;
    public GameObject Prefab;
}
[Serializable]
public class CornerCollection
{
    public Direction Place;
    public List<CornerPrefabHandler> Handlers;
    public void SetCorner(TileCornerType type)
    {
        foreach (var handler in Handlers)
        {
            if (handler.Type == type)
                handler.Prefab.SetActive(true);
            else
                handler.Prefab.SetActive(false);
        }
    }
}

[SelectionBase]
public class Tile : MonoBehaviour
{
    [SerializeField] public TileElementsHandler elementsHandler;
    [SerializeField] public List<WallCollection> walls = new();
    [SerializeField] public List<CornerCollection> corners = new();
    [SerializeField] public Material transparentMaterial;
    [SerializeField] public Material errorMaterial;
    [SerializeField] public Material startMaterial;

    [SerializeField] BuilderMatrix builderMatrix;
    [SerializeField] WallSolver wallSolver;

    [SerializeField] List<string> marks;
    [SerializeField] Vector2Int position = new(0,0);
    [SerializeField] int rotation = 0;

    public float SelectLiftingHeight = 3;
    float UnSelectedYPosition;
    float SelectedYPosition;
    
    Dictionary<Direction, List<TileWallType>> cachedWalls;

    Renderer[] renderers;
    TileState currentState = TileState.Normal;

    public Vector2Int Position { get => position; set => SetPosition(value); }
    public int Rotation { get => rotation; set => SetRotation(value); }
    public IEnumerable<string> Marks { get => marks; }
    public Dictionary<Direction, List<TileWallType>> Walls { 
        get {
            if (cachedWalls == null)
            {
                UpdateWallsCache();
            }
            return cachedWalls; 
        } 
    }

    public void Awake()
    {
        SetActileChilds(transform);
        renderers = GetComponentsInChildren<Renderer>();
        UnSelectedYPosition = transform.position.y;
        SelectedYPosition = UnSelectedYPosition + SelectLiftingHeight;
    }
    public void SetActileChilds(Transform transform)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(true);
            SetActileChilds(child);
        }
    }

    public void Init(BuilderMatrix builderMatrix, WallSolver wallSolver)
    {
        this.builderMatrix = builderMatrix;
        this.wallSolver = wallSolver;
    }

    public Result RequestWallUpdates(Dictionary<Direction, Tile> neighbours)
    {
        Dictionary<Direction, TileWallType> configuration = new();
        foreach (var direction in Direction.Up.GetCircle90())
        {
            TileWallType? wallTypeToPlace = TileWallType.none;
            var outTile = neighbours[direction];
            if (outTile != null)
            {
                wallTypeToPlace = wallSolver.ChooseWall(
                    Marks,
                    Walls[direction],
                    outTile.Marks,
                    outTile.Walls[direction.GetOpposite()]
                );
            }
            if (wallTypeToPlace == null)
            {
                return new FailResult("No walls to place");
            }
            configuration.Add(direction, wallTypeToPlace.Value);
        }
        return new SuccessResult<Dictionary<Direction, TileWallType>>(configuration);
    }
    public void ApplyUpdatingWalls(Result<Dictionary<Direction, TileWallType>> result)
    {
        foreach (var pair in result.Data)
        {
            GetWallCollection(pair.Key).SetWall(pair.Value);
        }
    }
    WallCollection GetWallCollection(Direction imaginePlace)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
        return walls.Find(x => x.Place == imaginePlace);
    }

    public void UpdateCorners(Dictionary<Direction, Tile> neighbours)
    {
        foreach (var direction in Direction.Left.GetCircle90())
        {
            TileCornerType cornerTypeToPlace = TileCornerType.none;
            var tile1 = neighbours[direction];
            var tile2 = neighbours[direction.Rotate45()];
            var tile3 = neighbours[direction.Rotate90()];
            if (tile1 != null && tile2 != null && tile3 != null)
            {
                cornerTypeToPlace = ChooseCorner(
                    IsWall(direction),
                    tile1.IsWall(direction.Rotate90()),
                    tile2.IsWall(direction.GetOpposite()),
                    tile3.IsWall(direction.RotateMinus90())
                    );
            }
            GetCornerCollection(direction.Rotate45()).SetCorner(cornerTypeToPlace);
        }
    }
    TileCornerType ChooseCorner(bool existWall1, bool existWall2, bool existWall3, bool existWall4)
    {
        if (existWall1 == true && existWall4 == true)
            return TileCornerType.inside;
        else if (existWall1 == true && existWall3 == true)
            return TileCornerType.wall_right;
        else if (existWall4 == true && existWall2 == true)
            return TileCornerType.wall_left;
        else if (existWall2 == true && existWall3 == true)
            return TileCornerType.outside_middle;
        else if (existWall1 == true && existWall2 == true)
            return TileCornerType.outside_right;
        else if (existWall4 == true && existWall3 == true)
            return TileCornerType.outside_left;
        else 
            return TileCornerType.none;
    }
    CornerCollection GetCornerCollection(Direction imaginePlace)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
        return corners.Find(x => x.Place == imaginePlace);
    }

    enum TileState
    {
        Normal,
        Selected,
        Errored,
        SelectedAndErrored
    }
    public void ApplySelectState()
    {
        if (currentState == TileState.Normal)
        {
            SetTileState(TileState.Selected);
        }
        else if (currentState == TileState.Errored)
        {
            SetTileState(TileState.SelectedAndErrored);
        }
    }
    public void CancelSelectState()
    {
        if(currentState == TileState.Selected)
        {
            SetTileState(TileState.Normal);
        }
        else if(currentState == TileState.SelectedAndErrored)
        {
            SetTileState(TileState.Errored);
        }
    }
    public void ApplyErrorState()
    {
        if (currentState == TileState.Normal)
        {
            SetTileState(TileState.Errored);
        }
        else if (currentState == TileState.Selected)
        {
            SetTileState(TileState.SelectedAndErrored);
        }
    }
    public void CancelErrorState()
    {
        if (currentState == TileState.Errored)
        {
            SetTileState(TileState.Normal);
        }
        else if (currentState == TileState.SelectedAndErrored)
        {
            SetTileState(TileState.Selected);
        }
    }

    void SetTileState(TileState state)
    {
        currentState = state;
        switch (currentState)
        {
            default:
            case TileState.Normal:
                transform.position = new Vector3(transform.position.x, UnSelectedYPosition, transform.position.z);
                foreach (var render in renderers)
                {
                    render.materials = new Material[1] { startMaterial };
                }
                break;
            case TileState.Selected:
                transform.position = new Vector3(transform.position.x, SelectedYPosition, transform.position.z);
                foreach (var render in renderers)
                {
                    render.materials = new Material[1] { transparentMaterial };
                }
                break;
            case TileState.Errored:
                transform.position = new Vector3(transform.position.x, UnSelectedYPosition, transform.position.z);
                foreach (var render in renderers)
                {
                    render.materials = new Material[2] { startMaterial, errorMaterial };
                }
                break;
            case TileState.SelectedAndErrored:
                transform.position = new Vector3(transform.position.x, SelectedYPosition, transform.position.z);
                foreach (var render in renderers)
                {
                    render.materials = new Material[2] { transparentMaterial, errorMaterial };
                }
                break;
        }
    }

    public void SetPosition(Vector2Int position)
    {
        this.position = position;
        if (builderMatrix != null)
        {
            transform.localPosition = new Vector3(
                builderMatrix.Step * position.y,
                transform.localPosition.y,
                -builderMatrix.Step * position.x
                );
        }
    }
    void SetRotation(int rotation)
    {
        this.rotation = rotation % 4;
        transform.rotation = Quaternion.Euler(0, 90 * this.rotation, 0);
        UpdateWallsCache();
    }
    bool IsWall(Direction imaginePlace)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
        var wallCollection = walls.Find(x => x.Place == imaginePlace);
        return !wallCollection.IsActive(TileWallType.none);
    }
    void UpdateWallsCache()
    {
        Dictionary<Direction, List<TileWallType>> list = new();
        foreach (var wall in walls)
        {
            Direction imagine_place = wall.Place;
            Enumerable.Range(0, rotation).ToList().ForEach(arg => imagine_place = imagine_place.Rotate90());
            list.Add(imagine_place, wall.Handlers.Select(x => x.Type).ToList());
        }
        cachedWalls = list;
    }
}