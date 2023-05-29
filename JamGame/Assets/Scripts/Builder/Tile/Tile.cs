using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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
    public TileWallType ActiveWallType { get; private set; }
    public void SetWall(TileWallType type)
    {
        ActiveWallType = type;
        Handlers.ForEach(x => x.Prefab.SetActive(x.Type == type));
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
        Handlers.ForEach(x => x.Prefab.SetActive(x.Type == type));
    }
}

[SelectionBase]
[RequireComponent(typeof(TileView))]
[RequireComponent(typeof(BoxCollider))]
public class Tile : MonoBehaviour
{
    [SerializeField] private BuilderMatrix builderMatrix;
    [SerializeField] private WallSolver wallSolver;

    [SerializeField] private List<string> marks;
    [SerializeField] private Vector2Int position = new(0, 0);
    [SerializeField, Range(0, 3)] private int rotation = 0;

    public TileElementsHandler ElementsHandler;
    public List<WallCollection> RawWalls = new();
    public List<CornerCollection> Corners = new();
    public TileView TileView;

    private Dictionary<Direction, List<TileWallType>> cachedWalls;
    private TileState currentState = TileState.Normal;

    public Vector2Int Position => position;
    public int Rotation => rotation;
    public IEnumerable<string> Marks => marks;

    private Dictionary<Direction, List<TileWallType>> walls
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
            TileWallType? wallTypeToPlace = TileWallType.None;
            Tile outTile = neighbours[direction];
            if (outTile != null)
            {
                wallTypeToPlace = wallSolver.ChooseWall(
                    Marks,
                    walls[direction],
                    outTile.Marks,
                    outTile.walls[direction.GetOpposite()]
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

    private WallCollection GetWallCollection(Direction imagine_place)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imagine_place = imagine_place.RotateMinus90());
        return RawWalls.Find(x => x.Place == imagine_place);
    }

    public void UpdateCorners(Dictionary<Direction, Tile> neighbours)
    {
        foreach (Direction direction in Direction.Left.GetCircle90())
        {
            TileCornerType corner_type_to_place = TileCornerType.None;
            Tile tile1 = neighbours[direction];
            Tile tile2 = neighbours[direction.Rotate45()];
            Tile tile3 = neighbours[direction.Rotate90()];
            if (tile1 != null && tile2 != null && tile3 != null)
            {
                corner_type_to_place = ChooseCorner(new TileWallType[] {
                    GetActiveWallType(direction),
                    tile1.GetActiveWallType(direction.Rotate90()),
                    tile2.GetActiveWallType(direction.GetOpposite()),
                    tile3.GetActiveWallType(direction.RotateMinus90())
                });
            }
            GetCornerCollection(direction.Rotate45()).SetCorner(corner_type_to_place);
        }
    }

    // Walls means:
    // 0 - own left wall
    // 1 - left neighbour up wall
    // 2 - left up neighbour right wall
    // 3 - up neigbour down wall
    private TileCornerType ChooseCorner(TileWallType[] walls)
    {
        Contract.Requires(walls.Length == 4); // HOW
        return (walls[0].IsWall(), walls[1].IsWall(), walls[2].IsWall(), walls[3].IsWall()) switch
        {
            (true, _, _, true) => TileCornerType.Inside,
            (true, _, true, _) => TileCornerType.WallRight,
            (_, true, _, true) => TileCornerType.WallLeft,
            (_, true, true, _) => TileCornerType.OutsideMiddle,
            (true, true, _, _) => TileCornerType.OutsideRight,
            (_, _, true, true) => TileCornerType.OutsideLeft,
            _ => TileCornerType.None,
        };
    }

    private CornerCollection GetCornerCollection(Direction imaginePlace)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
        return Corners.Find(x => x.Place == imaginePlace);
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
                TileView.SetMaterial(TileView.State.Default);
                break;
            case TileState.Selected:
                transform.position = new Vector3(transform.position.x, selectedYPosition, transform.position.z);
                TileView.SetMaterial(TileView.State.Selected);
                break;
            case TileState.SelectedAndErrored:
                transform.position = new Vector3(transform.position.x, selectedYPosition, transform.position.z);
                TileView.SetMaterial(TileView.State.SelectedOverlapping);
                break;
        }
    }
    public void SetPosition(Vector2Int _position)
    {
        position = _position;
        transform.localPosition = new Vector3(
            builderMatrix.Step * _position.y,
            transform.localPosition.y,
            -builderMatrix.Step * _position.x
            );
    }

    public void SetRotation(int _rotation)
    {
        rotation = _rotation < 0 ? (_rotation % 4) + 4 : _rotation % 4;
        transform.rotation = Quaternion.Euler(0, 90 * rotation, 0);
        CreateWallsCache();
    }

    public IEnumerable<Direction> GetPassableDirections()
    {
        foreach (Direction imagine_place in Direction.Up.GetCircle90())
        {
            Direction place = imagine_place;
            Enumerable.Range(0, rotation).ToList().ForEach(x => place = place.RotateMinus90());
            if (RawWalls.Find(x => x.Place == place).ActiveWallType.IsPassable())
            {
                yield return imagine_place;
            }
        }
    }

    private TileWallType GetActiveWallType(Direction imagine_place)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imagine_place = imagine_place.RotateMinus90());
        WallCollection wall_collection = RawWalls.Find(x => x.Place == imagine_place);
        return wall_collection.ActiveWallType;
    }

    private void CreateWallsCache()
    {
        Dictionary<Direction, List<TileWallType>> list = new();
        foreach (WallCollection wall in RawWalls)
        {
            Direction imagine_place = wall.Place;
            Enumerable.Range(0, rotation).ToList().ForEach(arg => imagine_place = imagine_place.Rotate90());
            list.Add(imagine_place, wall.Handlers.Select(x => x.Type).ToList());
        }
        cachedWalls = list;
    }
}