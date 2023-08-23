using Common;
using Level.Room;
using Pickle;
using System;
using System.Collections.Generic;
using System.Linq;
using TileUnion;
using TileUnion.Tile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

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

        [SerializeField]
        private AssetLabelReference tileUnionReference;

        public GameObject RootObject
        {
            get => rootObject;
            set => rootObject = value;
        }

        public Matrix BuilderMatrix => builderMatrix;

        public Dictionary<Vector2Int, TileUnionImpl> TileUnionDictionary { get; } = new();

        private Dictionary<string, IResourceLocation> modelViewMap = new();

        public Dictionary<string, TileUnionImpl> InstantiatedViews { get; } = new();

        public event Action<TileUnionImpl> OnTileUnionCreated;

        private Vector2Int stashPosition = new(-10, -10);

        private void Start()
        {
            UpdateModelViewMap();
        }

        public void UpdateModelViewMap()
        {
            modelViewMap.Clear();
            foreach (TileUnionImpl view in InstantiatedViews.Values)
            {
                DestroyImmediate(view.gameObject);
            }
            InstantiatedViews.Clear();

            foreach (
                LocationLinkPair<TileUnionImpl> pair in AddressablesTools.LoadAllFromLabel<TileUnionImpl>(
                    tileUnionReference
                )
            )
            {
                modelViewMap.Add(pair.Link.CoreModel.HashCode, pair.ResourceLocation);
                InstantiatedViews.Add(
                    pair.Link.CoreModel.HashCode,
                    Instantiate(pair.Link, fakeRootObject.transform)
                );

                TileUnionImpl unionInstance = InstantiatedViews.Last().Value;
                unionInstance.Constructor(() => null, BuilderMatrix);
                unionInstance.SetColliderActive(false);
                unionInstance.SetPosition(stashPosition);
                unionInstance.ApplyTileUnionState(TileImpl.TileState.Selected);
                unionInstance.IsolateUpdate();
            }
            ResetFakeViews();
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
            TileUnionImpl fakeTileUnion = InstantiatedViews[coreModel.HashCode];
            fakeTileUnion.gameObject.SetActive(true);
            fakeTileUnion.SetPositionAndRotation(coreModel.TileUnionModel.PlacingProperties);
            Result result = fakeTileUnion.IsValidPlacing(this);
            ResetFakeViews();
            return result.Failure ? result : new SuccessResult();
        }

        public void DropTileUnion(CoreModel coreModel)
        {
            List<Vector2Int> placingPositions = InstantiatedViews[coreModel.HashCode]
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
            ResetFakeViews();
            TileUnionImpl fakeTileUnion = InstantiatedViews[coreModel.HashCode];
            fakeTileUnion.gameObject.SetActive(true);
            fakeTileUnion.SetPositionAndRotation(coreModel.TileUnionModel.PlacingProperties);
            Result result = fakeTileUnion.IsValidPlacing(this);
            if (
                result.Failure
                || fakeTileUnion.TilesPositions
                    .Select(x => !GetTileUnionInPosition(x).IsAllWithMark("Freespace"))
                    .All(x => x)
            )
            {
                fakeTileUnion.ApplyTileUnionState(TileImpl.TileState.SelectedAndErrored);
            }
        }

        public CoreModel BorrowTileUnion(Vector2Int borrowedPosition)
        {
            ResetFakeViews();
            TileUnionImpl tileUnion = GetTileUnionInPosition(borrowedPosition);
            List<Vector2Int> previousPlaces = tileUnion.TilesPositions.ToList();
            CoreModel coreModel = DeleteTile(tileUnion);
            foreach (Vector2Int position in previousPlaces)
            {
                FreeSpace.TileUnionModel.PlacingProperties.SetPosition(position);
                CreateTileAndBind(FreeSpace);
            }
            UpdateSidesInPositions(previousPlaces);
            return coreModel;
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

        public void CreateTileAndBind(CoreModel coreModel)
        {
            TileUnionImpl tileUnion = CreateTile(coreModel);
            foreach (Vector2Int pos in tileUnion.TilesPositions)
            {
                TileUnionDictionary.Add(pos, tileUnion);
            }
            UpdateSidesInPositions(tileUnion.TilesPositionsForUpdating);
            OnTileUnionCreated?.Invoke(tileUnion);
        }

        private TileUnionImpl CreateTile(CoreModel coreModel)
        {
            if (modelViewMap.TryGetValue(coreModel.HashCode, out IResourceLocation location))
            {
                TileUnionImpl tileUnion = Instantiate(
                    AddressablesTools.LoadAsset<TileUnionImpl>(location),
                    rootObject.transform
                );
                tileUnion.Constructor(() => coreModel, BuilderMatrix);
                tileUnion.SetPositionAndRotation(coreModel.TileUnionModel.PlacingProperties);
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
            CoreModel coreModel = tileUnion.GetCoreModel();
            coreModel.TileUnionModel.PlacingProperties.SetRotation(tileUnion.Rotation);
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

        public BuildingConfig SaveBuildingIntoConfig()
        {
            List<TileConfig> tileConfigs = new();

            foreach (TileUnionImpl tileUnion in TileUnionDictionary.Values.Distinct())
            {
                tileConfigs.Add(
                    new TileConfig(
                        tileUnion.GetCoreModel().HashCode,
                        tileUnion.Position,
                        tileUnion.Rotation
                    )
                );
            }

            BuildingConfig buildingConfig = BuildingConfig.CreateInstance(tileConfigs);

            return buildingConfig;
        }
    }
}
