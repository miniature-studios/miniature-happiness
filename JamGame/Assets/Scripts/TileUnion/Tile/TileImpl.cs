using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using TileBuilder;
using UnityEditor;
using UnityEngine;

namespace TileUnion.Tile
{
    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(View))]
    [RequireComponent(typeof(BoxCollider))]
    [AddComponentMenu("TileUnion.Tile.Tile")]
    public class TileImpl : MonoBehaviour
    {
        [SerializeField]
        private Matrix builderMatrix;

        [SerializeField]
        private WallSolver wallSolver;

        [SerializeField]
        private List<string> marks;

        [SerializeField]
        private Vector2Int position = new(0, 0);

        [SerializeField, Range(0, 3)]
        private int rotation = 0;

        public List<WallPrefabHandler> WallPrefabHandlers;
        public List<CornerPrefabHandler> CornerPrefabHandlers;

        public List<WallCollection> RawWalls = new();
        public List<CornerCollection> Corners = new();
        public View TileView;

        [SerializeField]
        private Dictionary<Direction, List<WallType>> cachedWalls;

        [SerializeField, InspectorReadOnly]
        private TileState currentState = TileState.Normal;

        public Vector2Int Position => position;
        public int Rotation => rotation;
        public IEnumerable<string> Marks => marks;

        private Dictionary<Direction, List<WallType>> Walls
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

        private void OnDrawGizmos()
        {
            /*
            Handles.Label(
                transform.position + transform.TransformDirection(new Vector3(0, 0, -1) * 2),
                "Right"
            );
            Handles.Label(
                transform.position + transform.TransformDirection(new Vector3(0, 0, 1) * 2),
                "Left"
            );
            Handles.Label(
                transform.position + transform.TransformDirection(new Vector3(1, 0, 0) * 2),
                "Up"
            );
            Handles.Label(
                transform.position + transform.TransformDirection(new Vector3(-1, 0, 0) * 2),
                "Down"
            );
            */
        }

        public struct WallTypeMatch
        {
            public Dictionary<Direction, WallType> Data;

            public WallTypeMatch(Dictionary<Direction, WallType> data)
            {
                Data = data;
            }
        }

        public Result<WallTypeMatch> RequestWallUpdates(Dictionary<Direction, TileImpl> neighbours)
        {
            Dictionary<Direction, WallType> configuration = new();
            foreach (Direction direction in Direction.Up.GetCircle90())
            {
                WallType? wallTypeToPlace = WallType.None;
                TileImpl outTile = neighbours?[direction];
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
            foreach (KeyValuePair<Direction, WallType> pair in result.Data.Data)
            {
                GetWallCollection(pair.Key).SetWall(pair.Value);
            }
        }

        private WallCollection GetWallCollection(Direction imagine_place)
        {
            Enumerable
                .Range(0, rotation)
                .ToList()
                .ForEach(x => imagine_place = imagine_place.RotateMinus90());
            return RawWalls.Find(x => x.Place == imagine_place);
        }

        public void UpdateCorners(Dictionary<Direction, TileImpl> neighbours)
        {
            foreach (Direction direction in Direction.Left.GetCircle90())
            {
                CornerType corner_type_to_place = CornerType.None;
                TileImpl tile1 = neighbours[direction];
                TileImpl tile2 = neighbours[direction.Rotate45()];
                TileImpl tile3 = neighbours[direction.Rotate90()];
                if (tile1 != null && tile2 != null && tile3 != null)
                {
                    corner_type_to_place = ChooseCorner(
                        new WallType[]
                        {
                            GetActiveWallType(direction),
                            tile1.GetActiveWallType(direction.Rotate90()),
                            tile2.GetActiveWallType(direction.GetOpposite()),
                            tile3.GetActiveWallType(direction.RotateMinus90())
                        }
                    );
                }
                GetCornerCollection(direction.Rotate45()).SetCorner(corner_type_to_place);
            }
        }

        // Walls means:
        // 0 - own left wall
        // 1 - left neighbour up wall
        // 2 - left up neighbour right wall
        // 3 - up neigbour down wall
        private CornerType ChooseCorner(WallType[] walls)
        {
            Contract.Requires(walls.Length == 4); // HOW
            return (
                walls[0].IsWall(),
                walls[1].IsWall(),
                walls[2].IsWall(),
                walls[3].IsWall()
            ) switch
            {
                (true, _, _, true) => CornerType.Inside,
                (true, _, true, _) => CornerType.WallRight,
                (_, true, _, true) => CornerType.WallLeft,
                (_, true, true, _) => CornerType.OutsideMiddle,
                (true, true, _, _) => CornerType.OutsideRight,
                (_, _, true, true) => CornerType.OutsideLeft,
                _ => CornerType.None,
            };
        }

        private CornerCollection GetCornerCollection(Direction imaginePlace)
        {
            Enumerable
                .Range(0, rotation)
                .ToList()
                .ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
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
                    transform.position = new Vector3(
                        transform.position.x,
                        unselectedYPosition,
                        transform.position.z
                    );
                    TileView.SetMaterial(View.State.Default);
                    break;
                case TileState.Selected:
                    transform.position = new Vector3(
                        transform.position.x,
                        selectedYPosition,
                        transform.position.z
                    );
                    TileView.SetMaterial(View.State.Selected);
                    break;
                case TileState.SelectedAndErrored:
                    transform.position = new Vector3(
                        transform.position.x,
                        selectedYPosition,
                        transform.position.z
                    );
                    TileView.SetMaterial(View.State.SelectedOverlapping);
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

        private WallType GetActiveWallType(Direction imagine_place)
        {
            Enumerable
                .Range(0, rotation)
                .ToList()
                .ForEach(x => imagine_place = imagine_place.RotateMinus90());
            WallCollection wall_collection = RawWalls.Find(x => x.Place == imagine_place);
            return wall_collection.ActiveWallType;
        }

        public void CreateWallsCache()
        {
            Dictionary<Direction, List<WallType>> list = new();
            foreach (WallCollection wall in RawWalls)
            {
                Direction imagine_place = wall.Place;
                Enumerable
                    .Range(0, rotation)
                    .ToList()
                    .ForEach(arg => imagine_place = imagine_place.Rotate90());
                list.Add(imagine_place, wall.Handlers.Select(x => x.Type).ToList());
            }
            cachedWalls = list;
        }
    }

    public enum WallType
    {
        Window,
        Wall,
        Door,
        None
    }

    public static class WallTypeTools
    {
        public static bool IsPassable(this WallType wallType)
        {
            return wallType is WallType.None or WallType.Door;
        }

        public static bool IsWall(this WallType wallType)
        {
            return wallType is not WallType.None;
        }
    }

    public enum CornerType
    {
        Inside,
        WallLeft,
        WallRight,
        OutsideLeft,
        OutsideMiddle,
        OutsideRight,
        None
    }

    [Serializable]
    public class WallPrefabHandler
    {
        public WallType Type;
        public GameObject Prefab;
    }

    [Serializable]
    public class WallCollection
    {
        public Direction Place;
        public List<WallPrefabHandler> Handlers;
        public WallType ActiveWallType { get; private set; }

        public void SetWall(WallType type)
        {
            ActiveWallType = type;
            Handlers.ForEach(x => x.Prefab.SetActive(x.Type == type));
        }
    }

    [Serializable]
    public class CornerPrefabHandler
    {
        public CornerType Type;
        public GameObject Prefab;
    }

    [Serializable]
    public class CornerCollection
    {
        public Direction Place;
        public List<CornerPrefabHandler> Handlers;

        public void SetCorner(CornerType type)
        {
            Handlers.ForEach(x => x.Prefab.SetActive(x.Type == type));
        }
    }
}
