using Common;
using Level.Room;
using Pickle;
using System;
using System.Collections.Generic;
using System.Linq;
using TileUnion;
using TileUnion.Tile;
using UnityEngine;

namespace TileBuilder
{
    [AddComponentMenu("Scripts/TileBuilder.TileBuilder")]
    public partial class TileBuilderImpl : MonoBehaviour
    {
        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel FreeSpace;

        [SerializeField]
        private GameObject rootObject;

        [SerializeField]
        private GameObject fakeRootObject;

        [SerializeField]
        private Matrix builderMatrix;

        public GameObject RootObject
        {
            get => rootObject;
            set => rootObject = value;
        }

        public Matrix BuilderMatrix => builderMatrix;

        public Dictionary<Vector2Int, TileUnionImpl> TileUnionDictionary { get; } = new();

        public Dictionary<CoreModel, TileUnionImpl> ModelViewMap { get; } = new();

        private Dictionary<CoreModel, TileUnionImpl> instantiatedViews = new();

        public event Action<TileUnionImpl> OnTileUnionCreated;

        private Vector2Int stashPosition = new(-10, -10);

        private void Awake()
        {
            UpdateModelViewMap();
        }

        public void UpdateModelViewMap()
        {
            ModelViewMap.Clear();
            foreach (KeyValuePair<CoreModel, TileUnionImpl> view in instantiatedViews)
            {
                DestroyImmediate(view.Value.gameObject);
            }
            instantiatedViews.Clear();
            foreach (
                GameObject prefab in AddressableTools.GetAllAssetsByLabel(
                    AddressableTools.TileUnionsLabel
                )
            )
            {
                TileUnionImpl view = prefab.GetComponent<TileUnionImpl>();
                if (view != null)
                {
                    ModelViewMap.Add(view.CoreModel, view);
                    instantiatedViews.Add(
                        view.CoreModel,
                        Instantiate(view, fakeRootObject.transform).GetComponent<TileUnionImpl>()
                    );
                    instantiatedViews[view.CoreModel].Constructor(() => null);
                    instantiatedViews[view.CoreModel].SetColliderActive(false);
                    instantiatedViews[view.CoreModel].SetPosition(stashPosition);
                    instantiatedViews[view.CoreModel].ApplyTileUnionState(
                        TileImpl.TileState.Selected
                    );
                    instantiatedViews[view.CoreModel].IsolateUpdate();
                }
            }
        }

        public Result Validate()
        {
            Stack<KeyValuePair<Vector2Int, TileUnionImpl>> points_stack =
                new(TileUnionDictionary.Where(x => x.Value.IsAllWithMark("Door")));
            List<KeyValuePair<Vector2Int, TileUnionImpl>> tiles_to_check = TileUnionDictionary
                .Where(
                    x => !x.Value.IsAllWithMark("Outside") && !x.Value.IsAllWithMark("Freespace")
                )
                .ToList();

            while (points_stack.Count > 0)
            {
                KeyValuePair<Vector2Int, TileUnionImpl> point = points_stack.Pop();
                foreach (
                    Direction dir in point.Value.GetAccessibleDirectionsFromPosition(point.Key)
                )
                {
                    List<KeyValuePair<Vector2Int, TileUnionImpl>> near_tiles =
                        new(tiles_to_check.Where(x => x.Key == dir.ToVector2Int() + point.Key));
                    if (near_tiles.Count() > 0)
                    {
                        foreach (KeyValuePair<Vector2Int, TileUnionImpl> founded_tile in near_tiles)
                        {
                            _ = tiles_to_check.Remove(founded_tile);
                            points_stack.Push(founded_tile);
                        }
                    }
                }
            }

            if (tiles_to_check.Count > 0)
            {
                foreach (TileUnionImpl union in tiles_to_check.Select(x => x.Value).Distinct())
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

        public void ResetFakeViews()
        {
            foreach (KeyValuePair<CoreModel, TileUnionImpl> pair in instantiatedViews)
            {
                if (pair.Value.Position != stashPosition)
                {
                    pair.Value.SetPosition(stashPosition);
                    pair.Value.ApplyTileUnionState(TileImpl.TileState.Selected);
                }
            }
        }

        public Result DropTileUnion(CoreModel coreModel, Vector2Int position, int rotation)
        {
            ResetFakeViews();
            TileUnionImpl fakeTileUnion = instantiatedViews[coreModel];
            fakeTileUnion.SetPosition(position);
            fakeTileUnion.SetRotation(rotation);
            List<Vector2Int> placingPositions = fakeTileUnion.TilesPositions.ToList();
            Result result = fakeTileUnion.IsValidPlacing(this);
            ResetFakeViews();
            if (result.Failure)
            {
                return result;
            }
            else
            {
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
                CreateTileAndBind(coreModel, position, rotation);
                UpdateSidesInPositions(placingPositions);
                return new SuccessResult();
            }
        }

        public Result ShowTileUnionIllusion(CoreModel coreModel, Vector2Int position, int rotation)
        {
            ResetFakeViews();
            TileUnionImpl fakeTileUnion = instantiatedViews[coreModel];
            fakeTileUnion.SetPosition(position);
            fakeTileUnion.SetRotation(rotation);
            Result result = fakeTileUnion.IsValidPlacing(this);
            if (
                result.Failure
                || !fakeTileUnion.TilesPositions.All(
                    x =>
                        GetTileUnionsInPositions(new List<Vector2Int>() { x })
                            .All(x => x.IsAllWithMark("Freespace"))
                )
            )
            {
                fakeTileUnion.ApplyTileUnionState(TileImpl.TileState.SelectedAndErrored);
            }
            return new SuccessResult();
        }

        public Result BorrowTileUnion(TileUnionImpl tileUnion, Action<CoreModel> borrowedCoreModel)
        {
            ResetFakeViews();
            List<Vector2Int> previousPlaces = tileUnion.TilesPositions.ToList();
            borrowedCoreModel(DeleteTile(tileUnion));
            foreach (Vector2Int position in previousPlaces)
            {
                CreateTileAndBind(FreeSpace, position, 0);
            }
            UpdateSidesInPositions(previousPlaces);
            return new SuccessResult();
        }

        public void UpdateAllTiles()
        {
            foreach (KeyValuePair<Vector2Int, TileUnionImpl> pair in TileUnionDictionary)
            {
                pair.Value.UpdateWalls(this, pair.Key);
            }
            foreach (KeyValuePair<Vector2Int, TileUnionImpl> pair in TileUnionDictionary)
            {
                pair.Value.UpdateCorners(this, pair.Key);
            }
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

        public IEnumerable<Vector2Int> GetAllInsideListPositions()
        {
            return TileUnionDictionary
                .Where(x => !x.Value.IsAllWithMark("Outside"))
                .Select(x => x.Key);
        }

        public void CreateTileAndBind(CoreModel coreModel, Vector2Int position, int rotation)
        {
            TileUnionImpl tileUnion = CreateTile(coreModel, position, rotation);
            foreach (Vector2Int pos in tileUnion.TilesPositions)
            {
                TileUnionDictionary.Add(pos, tileUnion);
            }
            UpdateSidesInPositions(tileUnion.TilesPositionsForUpdating);
            OnTileUnionCreated?.Invoke(tileUnion);
        }

        private TileUnionImpl CreateTile(CoreModel coreModel, Vector2Int position, int rotation)
        {
            TileUnionImpl tileUnion = Instantiate(ModelViewMap[coreModel], rootObject.transform);
            tileUnion.Constructor(() => coreModel);
            tileUnion.SetPosition(position);
            tileUnion.SetRotation(rotation);
            return tileUnion;
        }

        private CoreModel DeleteTile(TileUnionImpl tileUnion)
        {
            CoreModel coreModel = tileUnion.GetCoreModel();
            coreModel.SetPlacingProperties(tileUnion.Rotation);
            RemoveTileFromDictionary(tileUnion);
            DestroyImmediate(tileUnion.gameObject);
            return coreModel;
        }

        public void DeleteAllTiles()
        {
            if (RootObject != null)
            {
                DestroyImmediate(RootObject);
            }
            RootObject = Instantiate(new GameObject("Root object"), transform);
            TileUnionDictionary.Clear();
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
            List<(TileUnionImpl, Vector2Int)> queue = new();
            foreach (Vector2Int position in positions)
            {
                if (TileUnionDictionary.TryGetValue(position, out TileUnionImpl tile))
                {
                    tile.UpdateWalls(this, position);
                    queue.Add((tile, position));
                }
            }
            foreach ((TileUnionImpl, Vector2Int) pair in queue)
            {
                pair.Item1.UpdateCorners(this, pair.Item2);
            }
        }

        public void LoadBuildingFromConfig(BuildingConfig buildingConfig)
        {
            foreach (TilePlaceConfig config in buildingConfig.TilePlaceConfigs)
            {
                CreateTileAndBind(config.CoreModel, config.Position, config.Rotation);
            }
        }

        public BuildingConfig SaveBuildingIntoConfig()
        {
            List<TilePlaceConfig> tilePlaceConfigs = new();

            foreach (TileUnionImpl tileUnion in TileUnionDictionary.Values.Distinct())
            {
                tilePlaceConfigs.Add(
                    new TilePlaceConfig(
                        tileUnion.GetCoreModel(),
                        tileUnion.Position,
                        tileUnion.Rotation
                    )
                );
            }

            BuildingConfig buildingConfig = BuildingConfig.CreateInstance(tilePlaceConfigs);

            return buildingConfig;
        }
    }
}
