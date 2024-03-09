using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Boss.Task;
using Level.Room;
using Pickle;
using TileBuilder.Command;
using TileUnion;
using TileUnion.Tile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace TileBuilder
{
    public enum GameMode
    {
        God,
        Build,
        Play
    }

    [AddComponentMenu("Scripts/TileBuilder/TileBuilder")]
    public partial class TileBuilderImpl : MonoBehaviour
    {
        private DataProvider<RoomCountByUid> roomCountDataProvider;

        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel FreeSpace;

        [SerializeField]
        private GridProperties gridProperties;

        [SerializeField]
        private AssetLabelReference tileUnionsLabel;

        [SerializeField]
        private List<CoreModel> coreModels = new();

        public IEnumerable<CoreModel> AllCoreModels => coreModels;

        private Validator.IValidator validator;

        public GridProperties GridProperties => gridProperties;

        public Dictionary<Vector2Int, TileUnionImpl> TileUnionDictionary { get; } = new();

        private Dictionary<InternalUid, IResourceLocation> modelViewMap = new();

        public Dictionary<InternalUid, TileUnionImpl> InstantiatedViews { get; } = new();

        public GameMode CurrentGameMode => validator.GameMode;

        private GameObject stashRootObject;
        private Vector2Int stashPosition = new(-10, -10);
        private float boundingBoxMin = -100.0f;
        private float boundingBoxMax = 100.0f;

        private void Awake()
        {
            stashRootObject = new GameObject("Stash Root Object");
            ChangeGameMode(GameMode.God);
            InitModelViewMap();
        }

        private void Start()
        {
            roomCountDataProvider = new DataProvider<RoomCountByUid>(() =>
            {
                Dictionary<InternalUid, int> count = new();

                foreach (CoreModel core_model in coreModels)
                {
                    if (count.ContainsKey(core_model.Uid))
                    {
                        count[core_model.Uid]++;
                    }
                    else
                    {
                        count.Add(core_model.Uid, 1);
                    }
                }

                return new RoomCountByUid() { CountByUid = count };
            });
        }

        public void ChangeGameMode(GameMode gameMode)
        {
            validator = gameMode switch
            {
                GameMode.God => new Validator.GodMode(this),
                GameMode.Build => new Validator.BuildMode(this),
                GameMode.Play => new Validator.GameMode(),
                _ => throw new ArgumentException(),
            };
        }

        private void InitModelViewMap()
        {
            modelViewMap.Clear();
            foreach (TileUnionImpl view in InstantiatedViews.Values)
            {
                DestroyImmediate(view.gameObject);
            }
            InstantiatedViews.Clear();

            foreach (
                AssetWithLocation<TileUnionImpl> tileUnion in AddressableTools<TileUnionImpl>.LoadAllFromLabel(
                    tileUnionsLabel
                )
            )
            {
                modelViewMap.Add(tileUnion.Asset.Uid, tileUnion.Location);
                InstantiatedViews.Add(
                    tileUnion.Asset.Uid,
                    Instantiate(tileUnion.Asset, stashRootObject.transform)
                );

                TileUnionImpl unionInstance = InstantiatedViews.Last().Value;
                unionInstance.SetGridProperties(gridProperties);
                unionInstance.CreateCache();
                unionInstance.SetColliderActive(false);
                unionInstance.SetPosition(stashPosition);
                unionInstance.ApplyTileUnionState(State.Selected);
                unionInstance.IsolateUpdate();
                unionInstance.gameObject.SetActive(false);
            }
            ResetStashedViews();
        }

        public Result ExecuteCommand(ICommand command)
        {
            Result response = validator.ValidateCommand(command);
            if (response.Failure)
            {
                return response;
            }

            if (command is DropRoom dropRoom)
            {
                coreModels.Add(dropRoom.CoreModel);
                dropRoom.CoreModel.transform.parent = transform;
            }
            else if (command is RemoveAllRooms)
            {
                foreach (CoreModel room in coreModels)
                {
                    Destroy(room.gameObject);
                }
                coreModels.Clear();
            }
            command.Execute(this);
            if (command is BorrowRoom borrowRoom)
            {
                _ = coreModels.Remove(borrowRoom.BorrowedRoom);
            }

            return new SuccessResult();
        }

        public Result Validate()
        {
            Stack<KeyValuePair<Vector2Int, TileUnionImpl>> pointsStack =
                new(TileUnionDictionary.Where(x => x.Value.IsAllWithMark(RoomTileLabel.Elevator)));
            List<KeyValuePair<Vector2Int, TileUnionImpl>> tilesToCheck = TileUnionDictionary
                .Where(x =>
                    !x.Value.IsAllWithMark(RoomTileLabel.Outside)
                    && !x.Value.IsAllWithMark(RoomTileLabel.FreeSpace)
                )
                .ToList();

            while (pointsStack.Count > 0)
            {
                KeyValuePair<Vector2Int, TileUnionImpl> point = pointsStack.Pop();
                foreach (
                    Direction dir in point.Value.GetAccessibleDirectionsFromPosition(point.Key)
                )
                {
                    List<KeyValuePair<Vector2Int, TileUnionImpl>> nearTiles =
                        new(tilesToCheck.Where(x => x.Key == dir.ToVector2Int() + point.Key));
                    if (nearTiles.Count() > 0)
                    {
                        foreach (KeyValuePair<Vector2Int, TileUnionImpl> foundedTile in nearTiles)
                        {
                            _ = tilesToCheck.Remove(foundedTile);
                            pointsStack.Push(foundedTile);
                        }
                    }
                }
            }

            if (tilesToCheck.Count > 0)
            {
                foreach (TileUnionImpl union in tilesToCheck.Select(x => x.Value).Distinct())
                {
                    union.ShowInvalidPlacing();
                }
                return new FailResult("Some tiles not connected");
            }
            else
            {
                return new SuccessResult();
            }
        }

        public void ResetStashedViews()
        {
            foreach (TileUnionImpl union in InstantiatedViews.Values)
            {
                if (union.Position != stashPosition)
                {
                    union.SetPosition(stashPosition);
                    union.ApplyTileUnionState(State.Selected);
                    union.gameObject.SetActive(false);
                }
            }
        }

        public Result IsValidPlacing(CoreModel coreModel)
        {
            TileUnionImpl stashTileUnion = InstantiatedViews[coreModel.Uid];
            stashTileUnion.gameObject.SetActive(true);
            stashTileUnion.ApplyPlacingProperties(coreModel.TileUnionModel.PlacingProperties);
            Result placingResult = stashTileUnion.IsValidPlacing(this);
            Result conditionResult = stashTileUnion.IsPassedConditions(
                coreModel.TileUnionModel.PlaceConditions,
                this
            );
            ResetStashedViews();
            return placingResult.Failure
                ? placingResult
                : conditionResult.Failure
                    ? conditionResult
                    : new SuccessResult();
        }

        public void InstantiateTileUnion(CoreModel coreModel)
        {
            TileUnionImpl tileUnion = CreateTile(coreModel);
            PlaceTileUnion(tileUnion);
        }

        public void PlaceTileUnion(TileUnionImpl tileUnion, bool changeParent = true)
        {
            List<Vector2Int> placingPositions = tileUnion
                .GetImaginePlaces(tileUnion.CoreModel.TileUnionModel.PlacingProperties)
                .ToList();

            List<TileUnionImpl> tilesUnder = TileUnionDictionary
                .Where(x => placingPositions.Contains(x.Key))
                .Select(x => x.Value)
                .ToList();

            for (int i = 0; i < tilesUnder.Count; i++)
            {
                _ = DeleteTile(tilesUnder[i]);
            }

            BindTileUnion(tileUnion, changeParent);
            UpdateSidesInPositions(placingPositions);
        }

        public void ShowSelectedTileUnion(CoreModel coreModel)
        {
            ResetStashedViews();
            TileUnionImpl stashTileUnion = InstantiatedViews[coreModel.Uid];
            stashTileUnion.gameObject.SetActive(true);
            stashTileUnion.ApplyPlacingProperties(coreModel.TileUnionModel.PlacingProperties);
            Result placingResult = stashTileUnion.IsValidPlacing(this);
            Result conditionResult = stashTileUnion.IsPassedConditions(
                coreModel.TileUnionModel.PlaceConditions,
                this
            );
            if (
                placingResult.Failure
                || conditionResult.Failure
                || stashTileUnion.TilesPositions.Any(x =>
                    GetTileUnionInPosition(x) == null
                    || !GetTileUnionInPosition(x).IsAllWithMark(RoomTileLabel.FreeSpace)
                )
            )
            {
                stashTileUnion.ApplyTileUnionState(State.SelectedAndErrored);
            }
        }

        public TileUnionImpl BorrowTileUnion(Vector2Int borrowedPosition)
        {
            ResetStashedViews();
            TileUnionImpl tileUnion = GetTileUnionInPosition(borrowedPosition);

            List<Vector2Int> previousPlaces = tileUnion.TilesPositions.ToList();
            RemoveTileFromDictionary(tileUnion);
            foreach (Vector2Int position in previousPlaces)
            {
                FreeSpace.TileUnionModel.PlacingProperties.SetPosition(position);
                TileUnionImpl freeSpace = CreateTile(FreeSpace);
                PlaceTileUnion(freeSpace, false);
            }
            UpdateSidesInPositions(previousPlaces);
            return tileUnion;
        }

        public CoreModel RemoveTileUnion(Vector2Int borrowedPosition)
        {
            TileUnionImpl removed = BorrowTileUnion(borrowedPosition);
            CoreModel coreModel = DeleteTile(removed);
            return removed.IsAllWithMark(RoomTileLabel.FreeSpace) ? null : coreModel;
        }

        public IEnumerable<TileUnionImpl> GetTileUnionsInPositions(
            IEnumerable<Vector2Int> positions
        )
        {
            return TileUnionDictionary
                .Where(x => positions.Contains(x.Key))
                .Select(x => x.Value)
                .Distinct();
        }

        public TileUnionImpl GetTileUnionInPosition(Vector2Int position)
        {
            return TileUnionDictionary.TryGetValue(position, out TileUnionImpl tileUnion)
                ? tileUnion
                : null;
        }

        public IEnumerable<Vector2Int> GetAllInsidePositions()
        {
            return TileUnionDictionary
                .Where(x => !x.Value.IsAllWithMark(RoomTileLabel.Outside))
                .Select(x => x.Key);
        }

        public IEnumerable<Vector2Int> GetAllPositions()
        {
            return TileUnionDictionary.Select(x => x.Key);
        }

        public void BindTileUnion(TileUnionImpl tileUnion, bool changeParent = true)
        {
            AddTileUnionToDictionary(tileUnion);
            UpdateSidesInPositions(tileUnion.TilesPositionsForUpdating);

#if UNITY_EDITOR
            OnTileUnionCreated?.Invoke(tileUnion);
#endif

            if (changeParent)
            {
                tileUnion.CoreModel.transform.SetParent(tileUnion.transform);
            }
        }

        private TileUnionImpl CreateTile(CoreModel coreModel)
        {
            if (modelViewMap.TryGetValue(coreModel.Uid, out IResourceLocation location))
            {
                TileUnionImpl tileUnion = Instantiate(
                    AddressableTools<TileUnionImpl>.LoadAsset(location),
                    transform
                );
                tileUnion.SetCoreModel(coreModel);
                tileUnion.SetGridProperties(gridProperties);
                tileUnion.CreateCache();
                tileUnion.ApplyPlacingProperties(coreModel.TileUnionModel.PlacingProperties);
                return tileUnion;
            }
            else
            {
                Debug.LogError($"Core model {coreModel.name} not presented in TileBuilder View");
                return null;
            }
        }

        private CoreModel DeleteTile(TileUnionImpl tileUnion)
        {
            CoreModel coreModel = tileUnion.CoreModel;
            coreModel.TileUnionModel.PlacingProperties.SetRotation(tileUnion.Rotation);
            RemoveTileFromDictionary(tileUnion);
            Destroy(tileUnion.gameObject);
            return coreModel;
        }

        public void DeleteAllTiles()
        {
            foreach (Transform child in transform)
            {
#if UNITY_EDITOR
                DestroyImmediate(child.transform.gameObject);
#else
                Destroy(child.transform.gameObject);
#endif
            }
            TileUnionDictionary.Clear();
        }

        private void AddTileUnionToDictionary(TileUnionImpl tileUnion)
        {
            foreach (Vector2Int pos in tileUnion.TilesPositions)
            {
                TileUnionDictionary.Add(pos, tileUnion);
            }
            RecalculateBounds();
        }

        private void RemoveTileFromDictionary(TileUnionImpl tileUnion)
        {
            bool flag;
            do
            {
                flag = false;
                foreach (KeyValuePair<Vector2Int, TileUnionImpl> item in TileUnionDictionary)
                {
                    if (item.Value == tileUnion)
                    {
                        _ = TileUnionDictionary.Remove(item.Key);
                        flag = true;
                        break;
                    }
                }
            } while (flag);
            RecalculateBounds();
        }

        private void UpdateSidesInPositions(IEnumerable<Vector2Int> positions)
        {
            List<(TileUnionImpl union, Vector2Int pos)> queue = new();
            foreach (Vector2Int position in positions)
            {
                if (TileUnionDictionary.TryGetValue(position, out TileUnionImpl tile))
                {
                    tile.UpdateWalls(this, position);
                    queue.Add((tile, position));
                }
            }
            foreach ((TileUnionImpl union, Vector2Int pos) in queue)
            {
                union.UpdateCorners(this, pos);
            }
        }

        public BuildingConfig SaveBuildingIntoConfig()
        {
            List<TileConfig> tileConfigs = new();

            foreach (TileUnionImpl tileUnion in TileUnionDictionary.Values.Distinct())
            {
                tileConfigs.Add(
                    new TileConfig(tileUnion.CoreModel.Uid, tileUnion.Position, tileUnion.Rotation)
                );
            }

            BuildingConfig buildingConfig = BuildingConfig.CreateInstance(tileConfigs);

            return buildingConfig;
        }

        public Bounds Bounds { get; private set; } =
            new Bounds(Vector3.zero, Vector3.positiveInfinity);

        private void RecalculateBounds()
        {
            IEnumerable<Vector2Int> insidePositions = TileUnionDictionary
                .Where(pair => !pair.Value.IsAllWithMark(RoomTileLabel.Outside))
                .Select(pair => pair.Key);

            if (!insidePositions.Any())
            {
                return;
            }

            Vector2Int minPoint = insidePositions.First();
            Vector2Int maxPoint = insidePositions.First();

            foreach (Vector2Int point in insidePositions)
            {
                minPoint = Vector2Int.Min(minPoint, point);
                maxPoint = Vector2Int.Max(maxPoint, point);
            }

            Vector3 point1 = GridProperties.GetWorldPoint(new(minPoint.x, minPoint.y));
            Vector3 point2 = GridProperties.GetWorldPoint(new(maxPoint.x, maxPoint.y));

            float halfStep = (float)GridProperties.Step / 2;
            Vector3 shift = new(halfStep, 0, halfStep);

            Vector3 min = Vector3.Min(point1, point2) - shift;
            min.y = boundingBoxMin;
            Vector3 max = Vector3.Max(point1, point2) + shift;
            max.y = boundingBoxMax;

            Bounds bounds = new();
            bounds.SetMinMax(min, max);
            Bounds = bounds;
        }
    }
}
