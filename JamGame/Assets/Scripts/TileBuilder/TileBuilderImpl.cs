using Common;
using Level.Room;
using Pickle;
using System;
using System.Collections.Generic;
using System.Linq;
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

    [AddComponentMenu("Scripts/TileBuilder.TileBuilder")]
    public partial class TileBuilderImpl : MonoBehaviour
    {
        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel FreeSpace;

        [SerializeField]
        private GameObject stashRootObject;

        [SerializeField]
        private GridProperties gridProperties;

        [SerializeField]
        private AssetLabelReference tileUnionsLabel;

        [SerializeField]
        private List<CoreModel> coreModels = new();

        private Validator.IValidator validator;

        public GridProperties GridProperties => gridProperties;

        public Dictionary<Vector2Int, TileUnionImpl> TileUnionDictionary { get; } = new();

        private Dictionary<string, IResourceLocation> modelViewMap = new();

        public Dictionary<string, TileUnionImpl> InstantiatedViews { get; } = new();

        public event Action<TileUnionImpl> OnTileUnionCreated;

        public GameMode CurrentGameMode => validator.GameMode;

        private Vector2Int stashPosition = new(-10, -10);

        private void Awake()
        {
            ChangeGameMode(GameMode.God);
            InitModelViewMap();
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
                unionInstance.ApplyTileUnionState(TileImpl.TileState.Selected);
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
                new(TileUnionDictionary.Where(x => x.Value.IsAllWithMark("Door")));
            List<KeyValuePair<Vector2Int, TileUnionImpl>> tilesToCheck = TileUnionDictionary
                .Where(
                    x => !x.Value.IsAllWithMark("Outside") && !x.Value.IsAllWithMark("Freespace")
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
                    union.ApplyTileUnionState(TileImpl.TileState.Selected);
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

        public void DropTileUnion(CoreModel coreModel)
        {
            List<Vector2Int> placingPositions = InstantiatedViews[coreModel.Uid]
                .GetImaginePlaces(coreModel.TileUnionModel.PlacingProperties)
                .ToList();

            List<TileUnionImpl> tilesUnder = TileUnionDictionary
                .Where(x => placingPositions.Contains(x.Key))
                .Select(x => x.Value)
                .ToList();

            while (tilesUnder.Count > 0)
            {
                TileUnionImpl buffer = tilesUnder.Last();
                _ = tilesUnder.Remove(buffer);
                _ = DeleteTile(buffer);
            }
            CreateTileAndBind(coreModel);
            UpdateSidesInPositions(placingPositions);
        }

        public void ShowSelectedTileUnion(CoreModel coreModel)
        {
            ResetStashedViews();
            TileUnionImpl stashTileUnion = InstantiatedViews[coreModel.Uid];
            stashTileUnion.gameObject.SetActive(true);
            stashTileUnion.ApplyPlacingProperties(coreModel.TileUnionModel.PlacingProperties);
            Result result = stashTileUnion.IsValidPlacing(this);
            if (
                result.Failure
                || stashTileUnion.TilesPositions.Any(
                    x =>
                        GetTileUnionInPosition(x) == null
                        || !GetTileUnionInPosition(x).IsAllWithMark("Freespace")
                )
            )
            {
                stashTileUnion.ApplyTileUnionState(TileImpl.TileState.SelectedAndErrored);
            }
        }

        public void BorrowTileUnion(Vector2Int borrowedPosition, out CoreModel borrowedRoom)
        {
            ResetStashedViews();
            TileUnionImpl tileUnion = GetTileUnionInPosition(borrowedPosition);
            List<Vector2Int> previousPlaces = tileUnion.TilesPositions.ToList();
            borrowedRoom = DeleteTile(tileUnion);
            foreach (Vector2Int position in previousPlaces)
            {
                FreeSpace.TileUnionModel.PlacingProperties.SetPosition(position);
                CreateTileAndBind(FreeSpace, false);
            }
            UpdateSidesInPositions(previousPlaces);
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
                .Where(x => !x.Value.IsAllWithMark("Outside"))
                .Select(x => x.Key);
        }

        public IEnumerable<Vector2Int> GetAllPositions()
        {
            return TileUnionDictionary.Select(x => x.Key);
        }

        public void CreateTileAndBind(CoreModel coreModel, bool changeParent = true)
        {
            TileUnionImpl tileUnion = CreateTile(coreModel);
            AddTileUnionToDictionary(tileUnion);
            UpdateSidesInPositions(tileUnion.TilesPositionsForUpdating);
            OnTileUnionCreated?.Invoke(tileUnion);
            if (changeParent)
            {
                coreModel.transform.SetParent(tileUnion.transform);
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
                Destroy(child.transform.gameObject);
            }
            TileUnionDictionary.Clear();
        }

        private void AddTileUnionToDictionary(TileUnionImpl tileUnion)
        {
            foreach (Vector2Int pos in tileUnion.TilesPositions)
            {
                TileUnionDictionary.Add(pos, tileUnion);
            }
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

        public Result CanGrowMeetingRoom(MeetingRoomLogics meetingRoom)
        {
            (_, IEnumerable<Vector2Int> positionsToTake, _) =
                meetingRoom.GetMeetingRoomGrowingInformation();
            foreach (Vector2Int position in positionsToTake)
            {
                TileUnionImpl targetTileUnion = GetTileUnionInPosition(position);
                if (
                    targetTileUnion
                        .GetAllUniqueMarks()
                        .Intersect(meetingRoom.IncorrectMarks)
                        .Count() > 0
                )
                {
                    return new FailResult("Cannot borrow tile with incorrect marks.");
                }
            }
            return new SuccessResult();
        }

        public List<CoreModel> GrowMeetingRoom(MeetingRoomLogics meetingRoom)
        {
            (
                IEnumerable<Vector2Int> movingTileUnionPositions,
                IEnumerable<Vector2Int> positionsToTake,
                Vector2Int movingDirection
            ) = meetingRoom.GetMeetingRoomGrowingInformation();

            IEnumerable<Vector2Int> positionToBorrow = positionsToTake.Where(
                x => GetTileUnionInPosition(x).GetAllUniqueMarks().All(x => x != "Freespace")
            );

            List<CoreModel> coreModels = new();

            foreach (Vector2Int position in positionToBorrow)
            {
                BorrowRoom command = new(position);
                _ = ExecuteCommand(command);
                coreModels.Add(command.BorrowedRoom);
            }

            foreach (Vector2Int position in positionsToTake)
            {
                _ = DeleteTile(GetTileUnionInPosition(position));
            }

            RemoveTileFromDictionary(meetingRoom.TileUnion);
            meetingRoom.AddTiles(movingDirection, movingTileUnionPositions);
            AddTileUnionToDictionary(meetingRoom.TileUnion);

            UpdateSidesInPositions(GetAllInsidePositions());

            return coreModels;
        }
    }
}
