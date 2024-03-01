using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Sirenix.OdinInspector;
using TileBuilder;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace TileUnion.Tile
{
    public enum State
    {
        Normal,
        Selected,
        Errored,
        SelectedAndErrored
    }

    [SelectionBase]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(View))]
    [RequireComponent(typeof(BoxCollider))]
    [AddComponentMenu("Scripts/TileUnion/Tile/TileUnion.Tile")]
    public partial class TileImpl : MonoBehaviour
    {
        [SerializeField]
        private WallSolver wallSolver;

        [SerializeField]
        private List<string> marks;

        [SerializeField]
        private Vector2Int position = new(0, 0);

        [Range(0, 3)]
        [SerializeField]
        private int rotation = 0;

        public List<WallPrefabHandler> WallPrefabHandlers;
        public List<CornerPrefabHandler> CornerPrefabHandlers;

        public List<WallCollection> RawWalls = new();
        public List<CornerCollection> Corners = new();
        public List<GameObject> CenterPrefabs = new();

        [SerializeField]
        private Dictionary<Direction, List<WallType>> cachedWalls;

        [ReadOnly]
        [SerializeField]
        private State currentState = State.Normal;

        public Vector2Int Position => position;
        public int Rotation => rotation;
        public IEnumerable<string> Marks => marks;

        [ReadOnly]
        [SerializeField]
        private List<TileImpl> projectedTiles = new();
        public IEnumerable<TileImpl> ProjectedTiles => projectedTiles;

        [SerializeField]
        private Transform projectedTilesRoot;

        [SerializeField]
        private GameObject tileToProject;
        public GameObject TileToProject => tileToProject;

        public UnityEvent<State> StateChanged;

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

        public struct WallTypeMatch
        {
            public Dictionary<Direction, WallType> Data;

            public WallTypeMatch(Dictionary<Direction, WallType> data)
            {
                Data = data;
            }
        }

        public Result<WallTypeMatch> RequestWallUpdates(Dictionary<Direction, TileImpl> neighbors)
        {
            Dictionary<Direction, WallType> configuration = new();
            foreach (Direction direction in Direction.Up.GetCircle90())
            {
                WallType? wallTypeToPlace = WallType.None;
                TileImpl outTile = neighbors?[direction];
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
            foreach (KeyValuePair<Direction, WallType> directionWall in result.Data.Data)
            {
                GetWallCollection(directionWall.Key).SetWall(directionWall.Value);
            }
            if (ProjectedTiles != null)
            {
                foreach (TileImpl tile in ProjectedTiles)
                {
                    tile.ApplyUpdatingWalls(result);
                }
            }
        }

        private WallCollection GetWallCollection(Direction imaginePlace)
        {
            Enumerable
                .Range(0, rotation)
                .ToList()
                .ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
            return RawWalls.Find(x => x.Place == imaginePlace);
        }

        private struct WallCross
        {
            public WallType Up;
            public WallType Down;
            public WallType Left;
            public WallType Right;

            public CornerType ChooseCorner()
            {
                return (Left.IsWall(), Up.IsWall(), Right.IsWall(), Down.IsWall()) switch
                {
                    (true, _, _, true) => CornerType.Inside,
                    (true, _, true, _) => CornerType.WallRight,
                    (_, true, _, true) => CornerType.WallLeft,
                    (_, true, true, _) => CornerType.OutsideMiddle,
                    (true, true, _, _) => CornerType.OutsideRight,
                    (_, _, true, true) => CornerType.OutsideLeft,
                    _ => CornerType.None
                };
            }

            public CornerType ChooseCornerForCorridor()
            {
                return (
                    Left.IsWallForCorridor(),
                    Up.IsWallForCorridor(),
                    Right.IsWallForCorridor(),
                    Down.IsWallForCorridor()
                ) switch
                {
                    (true, _, _, true) => CornerType.Inside,
                    (true, _, _, _) => CornerType.WallRight,
                    (_, _, _, true) => CornerType.WallLeft,
                    (false, _, _, false) => CornerType.OutsideMiddle,
                };
            }
        }

        public void UpdateCorners(Dictionary<Direction, TileImpl> neighbors)
        {
            foreach (Direction direction in Direction.Left.GetCircle90())
            {
                CornerType toPlace = CornerType.None;
                TileImpl tile1 = neighbors[direction];
                TileImpl tile2 = neighbors[direction.Rotate45()];
                TileImpl tile3 = neighbors[direction.Rotate90()];
                if (tile1 != null && tile2 != null && tile3 != null)
                {
                    WallCross wallCross =
                        new()
                        {
                            Left = GetActiveWallType(direction),
                            Up = tile1.GetActiveWallType(direction.Rotate90()),
                            Right = tile2.GetActiveWallType(direction.GetOpposite()),
                            Down = tile3.GetActiveWallType(direction.RotateMinus90())
                        };
                    toPlace = marks.Contains("Corridor")
                        ? wallCross.ChooseCornerForCorridor()
                        : wallCross.ChooseCorner();
                }
                GetCornerCollection(direction.Rotate45()).SetCorner(toPlace);
            }
            if (ProjectedTiles != null)
            {
                foreach (TileImpl tile in ProjectedTiles)
                {
                    tile.UpdateCorners(neighbors);
                }
            }
        }

        private CornerCollection GetCornerCollection(Direction imaginePlace)
        {
            Enumerable
                .Range(0, rotation)
                .ToList()
                .ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
            return Corners.Find(x => x.Place == imaginePlace);
        }

        public void SetTileState(State state)
        {
            if (state == currentState)
            {
                return;
            }
            currentState = state;
            StateChanged?.Invoke(currentState);
        }

        public void SetPosition(GridProperties gridProperties, Vector2Int newPosition)
        {
            position = newPosition;
            transform.localPosition = new Vector3(
                gridProperties.Step * newPosition.y,
                transform.localPosition.y,
                -gridProperties.Step * newPosition.x
            );
        }

        public void SetRotation(int newRotation)
        {
            rotation = ((newRotation % 4) + 4) % 4;
            transform.rotation = Quaternion.Euler(0, 90 * rotation, 0);
            CreateWallsCache();
        }

        public IEnumerable<Direction> GetPassableDirections()
        {
            foreach (Direction imaginePlace in Direction.Up.GetCircle90())
            {
                Direction place = imaginePlace;
                Enumerable.Range(0, rotation).ToList().ForEach(x => place = place.RotateMinus90());
                if (RawWalls.Find(x => x.Place == place).ActiveWallType.IsPassable())
                {
                    yield return imaginePlace;
                }
            }
        }

        private WallType GetActiveWallType(Direction imaginePlace)
        {
            Enumerable
                .Range(0, rotation)
                .ToList()
                .ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
            WallCollection wallCollection = RawWalls.Find(x => x.Place == imaginePlace);
            return wallCollection.ActiveWallType;
        }

        public void CreateWallsCache()
        {
            Dictionary<Direction, List<WallType>> list = new();
            foreach (WallCollection wall in RawWalls)
            {
                Direction imaginePlace = wall.Place;
                Enumerable
                    .Range(0, rotation)
                    .ToList()
                    .ForEach(arg => imaginePlace = imaginePlace.Rotate90());
                list.Add(imaginePlace, wall.Handlers.Select(x => x.Type).ToList());
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
            return wallType == WallType.None || wallType == WallType.Door;
        }

        public static bool IsWall(this WallType wallType)
        {
            return wallType != WallType.None;
        }

        public static bool IsWallForCorridor(this WallType wallType)
        {
            return wallType != WallType.None && wallType != WallType.Door;
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
