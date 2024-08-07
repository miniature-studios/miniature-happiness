using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Common;
using Level.Room;
using Pickle;
using Sirenix.OdinInspector;
using TileBuilder;
using TileUnion.PlaceCondition;
using TileUnion.Tile;
using UnityEngine;

namespace TileUnion
{
    [SelectionBase]
    [AddComponentMenu("Scripts/TileUnion/TileUnion")]
    public partial class TileUnionImpl : MonoBehaviour, IUidHandle
    {
        [Serializable]
        private class CachedConfiguration
        {
            public Vector2Int CenterTilePosition;
            public List<Vector2Int> TilesPositionsForUpdating;
            public List<Vector2Int> TilesPositions;
            public List<TileCachedConfiguration> TilesConfigurations;

            public CachedConfiguration(
                List<Vector2Int> tilesPositionsForUpdating,
                List<Vector2Int> tilesPositions,
                List<TileCachedConfiguration> tilesConfigurations,
                Vector2Int centerTilePosition
            )
            {
                TilesPositionsForUpdating = tilesPositionsForUpdating;
                TilesPositions = tilesPositions;
                TilesConfigurations = tilesConfigurations;
                CenterTilePosition = centerTilePosition;
            }
        }

        [Serializable]
        private class TileCachedConfiguration
        {
            public TileImpl TargetTile;
            public Vector2Int Position;
            public int Rotation;

            public TileCachedConfiguration(TileImpl targetTile, Vector2Int position, int rotation)
            {
                TargetTile = targetTile;
                Position = position;
                Rotation = rotation;
            }
        }

        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel CoreModelPrefab;

        public InternalUid Uid => CoreModelPrefab.Uid;

        [ReadOnly]
        [SerializeField]
        private CoreModel coreModel;
        public CoreModel CoreModel => coreModel;

        public void SetCoreModel(CoreModel coreModel)
        {
            this.coreModel = coreModel;
            coreModel.transform.SetParent(transform);
        }

        [Space(20)]
        [SerializeField]
        private Vector2Int position;

        [Range(0, 3)]
        [SerializeField]
        private int rotation;

        [ReadOnly]
        [SerializeField]
        private GridProperties gridProperties;

        private int selectedDuration = 1;

        public void SetGridProperties(GridProperties gridProperties)
        {
            this.gridProperties = gridProperties;
        }

        [SerializeField]
        private List<TileImpl> tiles = new();

        private Dictionary<int, CachedConfiguration> cachedConfiguration;
        private Dictionary<int, CachedConfiguration> Configuration => cachedConfiguration;
        public Vector2Int Position => position;
        public int Rotation => rotation;
        public IEnumerable<Vector2Int> TilesPositionsForUpdating =>
            Configuration[rotation].TilesPositionsForUpdating.Select(x => x + position);
        public IEnumerable<Vector2Int> TilesPositions =>
            Configuration[rotation].TilesPositions.Select(x => x + position);
        public int TilesCount => tiles.Count;

        public IEnumerable<RoomTileLabel> GetAllUniqueMarks()
        {
            return tiles
                .Select(x => x.Marks)
                .Aggregate(Enumerable.Empty<RoomTileLabel>(), (x, y) => x.Concat(y))
                .Distinct();
        }

        public bool IsAllWithMark(RoomTileLabel mark)
        {
            return tiles.Select(x => x.Marks.Contains(mark)).All(x => x == true);
        }

        public IEnumerable<Vector2Int> GetImaginePlaces(PlacingProperties placingProperties)
        {
            return Configuration[placingProperties.Rotation % 4].TilesPositions.Select(x =>
                x + placingProperties.Position
            );
        }

        public Result IsValidPlacingByWalls(TileBuilderImpl tileBuilder)
        {
            foreach (TileImpl tile in tiles)
            {
                Dictionary<Direction, TileImpl> neighbors = new();
                foreach (Direction pos in Direction.Up.GetCircle90())
                {
                    Vector2Int bufferPosition = Position + pos.ToVector2Int() + tile.Position;
                    _ = tileBuilder.TileUnionDictionary.TryGetValue(
                        bufferPosition,
                        out TileUnionImpl tileUnion
                    );
                    if (tileUnion != null)
                    {
                        neighbors.Add(
                            pos,
                            tileBuilder.TileUnionDictionary[bufferPosition].GetTile(bufferPosition)
                        );
                    }
                    else
                    {
                        neighbors.Add(pos, null);
                    }
                }
                if (tile.RequestWallUpdates(neighbors).Failure)
                {
                    return new FailResult($"Invalid walls on {tile.name} tile");
                }
            }
            return new SuccessResult();
        }

        public Result IsValidPlacingByConditioins(
            IEnumerable<IPlaceCondition> placeConditions,
            TileBuilderImpl tileBuilder
        )
        {
            foreach (IPlaceCondition placeCondition in placeConditions)
            {
                Result result = placeCondition.PassCondition(this, tileBuilder);
                if (result.Failure)
                {
                    return result;
                }
            }
            return new SuccessResult();
        }

        public void ApplyTileUnionState(State state)
        {
            foreach (TileImpl tile in tiles)
            {
                tile.SetTileState(state);
            }
        }

        public IEnumerable<Direction> GetAccessibleDirectionsFromPosition(Vector2Int position)
        {
            return tiles
                .FirstOrDefault(x => x.Position == position - this.position)
                .GetPassableDirections();
        }

        public void ShowInvalidPlacing()
        {
            foreach (TileImpl tile in tiles)
            {
                tile.SetTileState(State.Errored);
            }
            _ = StartCoroutine(ShowInvalidPlacingRoutine());
        }

        private IEnumerator ShowInvalidPlacingRoutine()
        {
            yield return new WaitForSecondsRealtime(selectedDuration);
            foreach (TileImpl tile in tiles)
            {
                tile.SetTileState(State.Normal);
            }
        }

        public void SetColliderActive(bool active)
        {
            foreach (TileImpl tile in tiles)
            {
                foreach (Collider collider in tile.GetComponentsInChildren<Collider>())
                {
                    collider.enabled = active;
                }
            }
        }

        public void UpdateWalls(TileBuilderImpl tileBuilder, Vector2Int position)
        {
            TileImpl tile = GetTile(position);
            Dictionary<Direction, TileImpl> neighbors = new();
            foreach (Direction pos in Direction.Up.GetCircle90())
            {
                Vector2Int bufferPosition = position + pos.ToVector2Int();
                _ = tileBuilder.TileUnionDictionary.TryGetValue(
                    bufferPosition,
                    out TileUnionImpl tileUnion
                );
                if (tileUnion != null)
                {
                    neighbors.Add(
                        pos,
                        tileBuilder.TileUnionDictionary[bufferPosition].GetTile(bufferPosition)
                    );
                }
                else
                {
                    neighbors.Add(pos, null);
                }
            }
            Result<TileImpl.WallTypeMatch> result = tile.RequestWallUpdates(neighbors);
            if (result.Success)
            {
                tile.ApplyUpdatingWalls(result);
            }
        }

        public void UpdateCorners(TileBuilderImpl tileBuilder, Vector2Int position)
        {
            TileImpl tile = GetTile(position);
            Dictionary<Direction, TileImpl> neighbors = new();
            foreach (Direction pos in Direction.Up.GetCircle45())
            {
                Vector2Int bufferPosition = position + pos.ToVector2Int();
                _ = tileBuilder.TileUnionDictionary.TryGetValue(
                    bufferPosition,
                    out TileUnionImpl tileUnion
                );
                if (tileUnion != null)
                {
                    neighbors.Add(
                        pos,
                        tileBuilder.TileUnionDictionary[bufferPosition].GetTile(bufferPosition)
                    );
                }
                else
                {
                    neighbors.Add(pos, null);
                }
            }
            tile.UpdateCorners(neighbors);
        }

        [Button(Style = ButtonStyle.Box)]
        public void IsolateUpdate()
        {
            foreach (TileImpl tile in tiles)
            {
                Dictionary<Direction, TileImpl> neighbors = new();
                foreach (Direction pos in Direction.Up.GetCircle90())
                {
                    Vector2Int bufferPosition = tile.Position + pos.ToVector2Int();
                    if (tiles.Select(x => x.Position).Contains(bufferPosition))
                    {
                        neighbors.Add(pos, tiles.FirstOrDefault(x => x.Position == bufferPosition));
                    }
                    else
                    {
                        neighbors.Add(pos, null);
                    }
                }
                Result<TileImpl.WallTypeMatch> result = tile.RequestWallUpdates(neighbors);
                if (result.Success)
                {
                    tile.ApplyUpdatingWalls(result);
                }
            }
        }

        [Title("Must be pressed before usage!")]
        [Button(Style = ButtonStyle.Box)]
        public void CreateCache(bool considerCenterOfMass = true)
        {
            cachedConfiguration = new();
            for (int i = 0; i < 4; i++)
            {
                List<TileCachedConfiguration> tileConfigurations = new();
                foreach (TileImpl tile in tiles)
                {
                    tileConfigurations.Add(new(tile, tile.Position, tile.Rotation));
                }
                cachedConfiguration.Add(
                    rotation,
                    new(
                        GetTilesPositionsForUpdating().ToList(),
                        tiles.Select(x => x.Position).ToList(),
                        tileConfigurations,
                        GetCenterTilePosition()
                    )
                );
                RotateTileUnion(considerCenterOfMass);
            }
        }

        [Button(Style = ButtonStyle.Box)]
        public void SetRotation(int rotation)
        {
            this.rotation = ((rotation % 4) + 4) % 4;
            foreach (
                TileCachedConfiguration config in Configuration[this.rotation].TilesConfigurations
            )
            {
                config.TargetTile.SetPosition(gridProperties, config.Position);
                config.TargetTile.SetRotation(config.Rotation);
            }
        }

        public void SetPosition(Vector2Int vector)
        {
            position = vector;
            transform.localPosition = new Vector3(
                gridProperties.Step * position.y,
                transform.localPosition.y,
                -gridProperties.Step * position.x
            );
        }

        public void ApplyPlacingProperties(PlacingProperties placingProperties)
        {
            SetRotation(placingProperties.Rotation);
            SetPosition(placingProperties.Position);
        }

        public IEnumerable<RoomTileLabel> GetTileMarks(Vector2Int globalPosition)
        {
            return GetTile(globalPosition).Marks;
        }

        private TileImpl GetTile(Vector2Int globalPosition)
        {
            globalPosition -= position;
            return tiles.FirstOrDefault(x => x.Position == globalPosition);
        }

        private void RotateTileUnion(bool considerCenterOfMass)
        {
            Vector2 firstCenter = Vector2.zero;
            if (considerCenterOfMass)
            {
                firstCenter = tiles.Select(x => x.Position).GetCenterOfMass();
            }

            rotation++;
            rotation %= 4;
            foreach (TileImpl tile in tiles)
            {
                tile.SetRotation(tile.Rotation + 1);
                tile.SetPosition(gridProperties, new Vector2Int(tile.Position.y, -tile.Position.x));
            }

            if (considerCenterOfMass)
            {
                Vector2 secondCenter = tiles.Select(x => x.Position).GetCenterOfMass();
                Vector2 delta = firstCenter - secondCenter;
                foreach (TileImpl tile in tiles)
                {
                    tile.SetPosition(
                        gridProperties,
                        tile.Position + new Vector2Int((int)delta.x, (int)delta.y)
                    );
                }
            }
        }

        private IEnumerable<Vector2Int> GetTilesPositionsForUpdating()
        {
            HashSet<Vector2Int> localPositions = new();
            foreach (TileImpl tile in tiles)
            {
                foreach (Direction position in Direction.Up.GetCircle45())
                {
                    Vector2Int pos = tile.Position + position.ToVector2Int();
                    _ = localPositions.Add(pos);
                }
                _ = localPositions.Add(tile.Position);
            }
            return localPositions;
        }

        private Vector2Int GetCenterTilePosition()
        {
            Vector2 vectorSum = new();
            foreach (Vector2Int pos in tiles.Select(x => x.Position))
            {
                vectorSum += pos;
            }
            vectorSum /= TilesCount;
            List<Vector2Int> vectors =
                new()
                {
                    new((int)Math.Truncate(vectorSum.x), (int)Math.Truncate(vectorSum.y)),
                    new(
                        (int)Math.Truncate(vectorSum.x),
                        (int)Math.Truncate(vectorSum.y) + (int)vectorSum.normalized.y
                    ),
                    new(
                        (int)Math.Truncate(vectorSum.x) + (int)vectorSum.normalized.x,
                        (int)Math.Truncate(vectorSum.y)
                    ),
                    new(
                        (int)Math.Truncate(vectorSum.x) + (int)vectorSum.normalized.x,
                        (int)Math.Truncate(vectorSum.y) + (int)vectorSum.normalized.y
                    )
                };
            return vectors.OrderBy(x => Vector2.Distance(x, vectorSum)).First();
        }

        public void MoveTiles(Vector2Int direction, IEnumerable<Vector2Int> movingPositions)
        {
            foreach (Vector2Int position in movingPositions)
            {
                TileImpl tile = GetTile(position);
                tile.SetPosition(gridProperties, tile.Position + direction);
            }
        }

        public void AddTiles(
            Dictionary<(Vector2Int globalPosition, int roatation), TileImpl> pairsToAdd
        )
        {
            foreach (
                KeyValuePair<
                    (Vector2Int globalPosition, int roatation),
                    TileImpl
                > pair in pairsToAdd
            )
            {
                TileImpl newTile = Instantiate(pair.Value, transform);
                tiles.Add(newTile);
                newTile.SetPosition(gridProperties, pair.Key.globalPosition - position);
                newTile.SetRotation(pair.Key.roatation);
            }
        }
    }
}
