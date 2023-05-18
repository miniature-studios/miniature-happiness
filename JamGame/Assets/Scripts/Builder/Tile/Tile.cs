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
        foreach (WallPrefabHandler handler in Handlers)
        {
            if (handler.Type == type)
            {
                handler.Prefab.SetActive(true);
            }
            else
            {
                handler.Prefab.SetActive(false);
            }
        }
    }
    public bool IsActive(TileWallType tileWallType)
    {
        return Handlers.Find(x => x.Type == tileWallType) != null
&& Handlers.Find(x => x.Type == tileWallType).Prefab.gameObject.activeSelf;
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
        foreach (CornerPrefabHandler handler in Handlers)
        {
            if (handler.Type == type)
            {
                handler.Prefab.SetActive(true);
            }
            else
            {
                handler.Prefab.SetActive(false);
            }
        }
    }
}

[SelectionBase]
[RequireComponent(typeof(TileView))]
[RequireComponent(typeof(BoxCollider))]
public class Tile : MonoBehaviour
{
    [SerializeField] public TileElementsHandler elementsHandler;
    [SerializeField] public List<WallCollection> walls = new();
    [SerializeField] public List<CornerCollection> corners = new();
    [SerializeField] public TileView tileView;

    [SerializeField] private BuilderMatrix builderMatrix;
    [SerializeField] private WallSolver wallSolver;

    [SerializeField] private List<string> marks;
    [SerializeField] private Vector2Int position = new(0, 0);
    [SerializeField, Range(0, 3)] private int rotation = 0;
    private Dictionary<Direction, List<TileWallType>> cachedWalls;
    private TileState currentState = TileState.Normal;

    public Vector2Int Position => position;
    public int Rotation => rotation;
    public IEnumerable<string> Marks => marks;
    public Dictionary<Direction, List<TileWallType>> Walls
    {
        get
        {
            if (cachedWalls == null)
            {
                CreateWallsCache();
            }
            return cachedWalls;
        }
    }

    private readonly float selectLiftingHeight = 3;
    private float unselectedYPosition;
    private float selectedYPosition;

    private void Awake()
    {
        unselectedYPosition = transform.position.y;
        selectedYPosition = unselectedYPosition + selectLiftingHeight;
    }

    private void OnValidate()
    {
        SetPosition(Position);
        SetRotation(Rotation);
    }

    public struct WallTypeMatch
    {
        public Dictionary<Direction, TileWallType> Data;
        public WallTypeMatch(Dictionary<Direction, TileWallType> data)
        {
            Data = data;
        }
    }

    public Result<WallTypeMatch> RequestWallUpdates(Dictionary<Direction, Tile> neighbours)
    {
        Dictionary<Direction, TileWallType> configuration = new();
        foreach (Direction direction in Direction.Up.GetCircle90())
        {
            TileWallType? wallTypeToPlace = TileWallType.none;
            Tile outTile = neighbours[direction];
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
                return new FailResult<WallTypeMatch>("No walls to place");
            }
            configuration.Add(direction, wallTypeToPlace.Value);
        }
        return new SuccessResult<WallTypeMatch>(new(configuration));
    }
    public void ApplyUpdatingWalls(Result<WallTypeMatch> result)
    {
        foreach (KeyValuePair<Direction, TileWallType> pair in result.Data.Data)
        {
            GetWallCollection(pair.Key).SetWall(pair.Value);
        }
    }

    private WallCollection GetWallCollection(Direction imaginePlace)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
        return walls.Find(x => x.Place == imaginePlace);
    }

    public void UpdateCorners(Dictionary<Direction, Tile> neighbours)
    {
        foreach (Direction direction in Direction.Left.GetCircle90())
        {
            TileCornerType cornerTypeToPlace = TileCornerType.none;
            Tile tile1 = neighbours[direction];
            Tile tile2 = neighbours[direction.Rotate45()];
            Tile tile3 = neighbours[direction.Rotate90()];
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

    private TileCornerType ChooseCorner(bool existWall1, bool existWall2, bool existWall3, bool existWall4)
    {
        return existWall1 == true && existWall4 == true
            ? TileCornerType.inside
            : existWall1 == true && existWall3 == true
                ? TileCornerType.wall_right
                : existWall4 == true && existWall2 == true
                            ? TileCornerType.wall_left
                            : existWall2 == true && existWall3 == true
                                        ? TileCornerType.outside_middle
                                        : existWall1 == true && existWall2 == true
                                                    ? TileCornerType.outside_right
                                                    : existWall4 == true && existWall3 == true ? TileCornerType.outside_left : TileCornerType.none;
    }

    private CornerCollection GetCornerCollection(Direction imaginePlace)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
        return corners.Find(x => x.Place == imaginePlace);
    }

    public enum TileState
    {
        Normal,
        Selected,
        SelectedAndErrored
    }
    public void SetTileState(TileState state)
    {
        currentState = state;
        switch (currentState)
        {
            default:
            case TileState.Normal:
                transform.position = new Vector3(transform.position.x, unselectedYPosition, transform.position.z);
                tileView.SetMaterial(TileView.TileMaterial.Default);
                break;
            case TileState.Selected:
                transform.position = new Vector3(transform.position.x, selectedYPosition, transform.position.z);
                tileView.SetMaterial(TileView.TileMaterial.Transparent);
                break;
            case TileState.SelectedAndErrored:
                transform.position = new Vector3(transform.position.x, selectedYPosition, transform.position.z);
                tileView.SetMaterial(TileView.TileMaterial.TransparentAndError);
                break;
        }
    }
    public void SetPosition(Vector2Int position)
    {
        this.position = position;
        transform.localPosition = new Vector3(
            builderMatrix.Step * position.y,
            transform.localPosition.y,
            -builderMatrix.Step * position.x
            );
    }
    public void SetRotation(int rotation)
    {
        this.rotation = rotation < 0 ? (rotation % 4) + 4 : rotation % 4;
        transform.rotation = Quaternion.Euler(0, 90 * this.rotation, 0);
        CreateWallsCache();
    }

    private bool IsWall(Direction imaginePlace)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
        WallCollection wallCollection = walls.Find(x => x.Place == imaginePlace);
        return !wallCollection.IsActive(TileWallType.none);
    }

    private void CreateWallsCache()
    {
        Dictionary<Direction, List<TileWallType>> list = new();
        foreach (WallCollection wall in walls)
        {
            Direction imagine_place = wall.Place;
            Enumerable.Range(0, rotation).ToList().ForEach(arg => imagine_place = imagine_place.Rotate90());
            list.Add(imagine_place, wall.Handlers.Select(x => x.Type).ToList());
        }
        cachedWalls = list;
    }
}