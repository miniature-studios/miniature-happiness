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

    public bool IsActive(TileWallType tile_wall_type)
    {
        return Handlers.Find(x => x.Type == tile_wall_type) != null
            && Handlers.Find(x => x.Type == tile_wall_type).Prefab.activeSelf;
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
    [SerializeField] private BuilderMatrix builderMatrix;
    [SerializeField] private WallSolver wallSolver;

    [SerializeField] private List<string> marks;
    [SerializeField] private Vector2Int position = new(0, 0);
    [SerializeField, Range(0, 3)] private int rotation = 0;

    public TileElementsHandler ElementsHandler;
    public List<WallCollection> Walls = new();
    public List<CornerCollection> Corners = new();
    public TileView TileView;

    private Dictionary<Direction, List<TileWallType>> cachedWalls;
    private TileState currentState = TileState.Normal;

    public Vector2Int Position => position;
    public int Rotation => rotation;
    public IEnumerable<string> Marks => marks;
    public Dictionary<Direction, List<TileWallType>> WallsDictionary
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
                    WallsDictionary[direction],
                    outTile.Marks,
                    outTile.WallsDictionary[direction.GetOpposite()]
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
        return Walls.Find(x => x.Place == imagine_place);
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
                corner_type_to_place = ChooseCorner(new(
                    IsWall(direction),
                    tile1.IsWall(direction.Rotate90()),
                    tile2.IsWall(direction.GetOpposite()),
                    tile3.IsWall(direction.RotateMinus90())
                    ));
            }
            GetCornerCollection(direction.Rotate45()).SetCorner(corner_type_to_place);
        }
    }

    private class WallsBools
    {
        public bool Wall1;
        public bool Wall2;
        public bool Wall3;
        public bool Wall4;
        public WallsBools(bool wall1, bool wall2, bool wall3, bool wall4)
        {
            Wall1 = wall1;
            Wall2 = wall2;
            Wall3 = wall3;
            Wall4 = wall4;
        }
    }
    private TileCornerType ChooseCorner(WallsBools wallsBools)
    {
        return wallsBools.Wall1 == true && wallsBools.Wall4 == true
            ? TileCornerType.Inside
            : wallsBools.Wall1 == true && wallsBools.Wall3 == true
                ? TileCornerType.WallRight
                : wallsBools.Wall4 == true && wallsBools.Wall2 == true
                            ? TileCornerType.WallLeft
                            : wallsBools.Wall2 == true && wallsBools.Wall3 == true
                                        ? TileCornerType.OutsideMiddle
                                        : wallsBools.Wall1 == true && wallsBools.Wall2 == true
                                                    ? TileCornerType.OutsideRight
                                                    : wallsBools.Wall4 == true && wallsBools.Wall3 == true ? TileCornerType.OutsideLeft : TileCornerType.None;
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
                TileView.SetMaterial(TileView.TileMaterial.Default);
                break;
            case TileState.Selected:
                transform.position = new Vector3(transform.position.x, selectedYPosition, transform.position.z);
                TileView.SetMaterial(TileView.TileMaterial.Transparent);
                break;
            case TileState.SelectedAndErrored:
                transform.position = new Vector3(transform.position.x, selectedYPosition, transform.position.z);
                TileView.SetMaterial(TileView.TileMaterial.TransparentAndError);
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

    private bool IsWall(Direction imagine_place)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imagine_place = imagine_place.RotateMinus90());
        WallCollection wall_collection = Walls.Find(x => x.Place == imagine_place);
        return !wall_collection.IsActive(TileWallType.None);
    }

    private void CreateWallsCache()
    {
        Dictionary<Direction, List<TileWallType>> list = new();
        foreach (WallCollection wall in Walls)
        {
            Direction imagine_place = wall.Place;
            Enumerable.Range(0, rotation).ToList().ForEach(arg => imagine_place = imagine_place.Rotate90());
            list.Add(imagine_place, wall.Handlers.Select(x => x.Type).ToList());
        }
        cachedWalls = list;
    }
}