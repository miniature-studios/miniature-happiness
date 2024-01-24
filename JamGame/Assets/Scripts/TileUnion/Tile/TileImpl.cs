using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Sirenix.OdinInspector;
using TileBuilder;
using UnityEditor;
using UnityEngine;

namespace TileUnion.Tile
{
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
        public View TileView;

        [SerializeField]
        private Dictionary<Direction, List<WallType>> cachedWalls;

        [ReadOnly]
        [SerializeField]
        private TileState currentState = TileState.Normal;

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
                    WallType[] wallTypes = new WallType[]
                    {
                        GetActiveWallType(direction),
                        tile1.GetActiveWallType(direction.Rotate90()),
                        tile2.GetActiveWallType(direction.GetOpposite()),
                        tile3.GetActiveWallType(direction.RotateMinus90())
                    };
                    toPlace = marks.Contains("Corridor")
                        ? ChooseCornerForCorridor(wallTypes)
                        : ChooseCorner(wallTypes);
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

        // Walls means:
        // 0 - own left wall
        // 1 - left neighbor up wall
        // 2 - left up neighbor right wall
        // 3 - up neighbor down wall
        private CornerType ChooseCorner(WallType[] walls)
        {
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

        private CornerType ChooseCornerForCorridor(WallType[] walls)
        {
            if (walls[0] is WallType.Door && walls[3] is WallType.Door)
            {
                return CornerType.OutsideMiddle;
            }
            return (
                walls[0].IsWallForCorridor(),
                walls[1].IsWallForCorridor(),
                walls[2].IsWallForCorridor(),
                walls[3].IsWallForCorridor()
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
            if (state == currentState)
            {
                return;
            }

            currentState = state;
            View.State viewState = currentState switch
            {
                TileState.Normal => View.State.Default,
                TileState.Selected => View.State.Selected,
                TileState.SelectedAndErrored => View.State.SelectedOverlapping,
                _ => throw new InvalidOperationException()
            };

            float newY = currentState switch
            {
                TileState.Normal => unselectedYPosition,
                TileState.Selected => selectedYPosition,
                TileState.SelectedAndErrored => selectedYPosition,
                _ => throw new InvalidOperationException()
            };
            transform.SetYPosition(newY);
            TileView.SetMaterial(viewState);
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
