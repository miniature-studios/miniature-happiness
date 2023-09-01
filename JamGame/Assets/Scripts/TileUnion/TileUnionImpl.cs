using Common;
using Level.Room;
using Pickle;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TileBuilder;
using TileUnion.PlaceCondition;
using TileUnion.Tile;
using UnityEngine;

namespace TileUnion
{
    [SelectionBase]
    [AddComponentMenu("Scripts/TileUnion.TileUnion")]
    public partial class TileUnionImpl : MonoBehaviour
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

        public string Uid => CoreModelPrefab.Uid;

        [SerializeField]
        [InspectorReadOnly]
        private CoreModel coreModel;
        public CoreModel CoreModel => coreModel;

        public void SetCoreModel(CoreModel coreModel)
        {
            this.coreModel = coreModel;
        }

        [Space(20)]
        [SerializeField]
        private Vector2Int position;

        [SerializeField, Range(0, 3)]
        private int rotation;

        [SerializeField]
        [InspectorReadOnly]
        private GridProperties gridProperties;

        public void SetGridProperties(GridProperties gridProperties)
        {
            this.gridProperties = gridProperties;
        }

        public List<TileImpl> Tiles = new();

        private Dictionary<int, CachedConfiguration> cachedConfiguration;
        private Dictionary<int, CachedConfiguration> Configuration => cachedConfiguration;
        public Vector2Int Position => position;
        public int Rotation => rotation;
        public IEnumerable<Vector2Int> TilesPositionsForUpdating =>
            Configuration[rotation].TilesPositionsForUpdating.Select(x => x + position);
        public IEnumerable<Vector2Int> TilesPositions =>
            Configuration[rotation].TilesPositions.Select(x => x + position);
        public int TilesCount => Tiles.Count;

        public IEnumerable<string> GetAllUniqueMarks()
        {
            return Tiles
                .Select(x => x.Marks)
                .Aggregate(Enumerable.Empty<string>(), (x, y) => x.Concat(y))
                .Distinct();
        }

        public bool IsAllWithMark(string mark)
        {
            return Tiles.Select(x => x.Marks.Contains(mark)).All(x => x == true);
        }

        public IEnumerable<Vector2Int> GetImaginePlaces(PlacingProperties placingProperties)
        {
            return Configuration[placingProperties.Rotation % 4].TilesPositions.Select(
                x => x + placingProperties.Position
            );
        }

        public Result IsValidPlacing(TileBuilderImpl tileBuilder)
        {
            foreach (TileImpl tile in Tiles)
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

        public Result IsPassedConditions(
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

        public void ApplyTileUnionState(TileImpl.TileState state)
        {
            foreach (TileImpl tile in Tiles)
            {
                tile.SetTileState(state);
            }
        }

        public IEnumerable<Direction> GetAccessibleDirectionsFromPosition(Vector2Int position)
        {
            return Tiles.Find(x => x.Position == position - this.position).GetPassableDirections();
        }

        public void ShowInvalidPlacing()
        {
            foreach (TileImpl tile in Tiles)
            {
                tile.SetTileState(TileImpl.TileState.SelectedAndErrored);
            }
            _ = StartCoroutine(ShowInvalidPlacingRoutine());
        }

        private IEnumerator ShowInvalidPlacingRoutine()
        {
            yield return new WaitForSeconds(1);
            foreach (TileImpl tile in Tiles)
            {
                tile.SetTileState(TileImpl.TileState.Normal);
            }
        }

        public void SetColliderActive(bool active)
        {
            foreach (TileImpl tile in Tiles)
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

        public void IsolateUpdate()
        {
            foreach (TileImpl tile in Tiles)
            {
                Dictionary<Direction, TileImpl> neighbors = new();
                foreach (Direction pos in Direction.Up.GetCircle90())
                {
                    Vector2Int bufferPosition = tile.Position + pos.ToVector2Int();
                    if (Tiles.Select(x => x.Position).Contains(bufferPosition))
                    {
                        neighbors.Add(pos, Tiles.FirstOrDefault(x => x.Position == bufferPosition));
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

        public void CreateCache()
        {
            cachedConfiguration = new();
            for (int i = 0; i < 4; i++)
            {
                List<TileCachedConfiguration> tileConfigurations = new();
                foreach (TileImpl tile in Tiles)
                {
                    tileConfigurations.Add(new(tile, tile.Position, tile.Rotation));
                }
                cachedConfiguration.Add(
                    rotation,
                    new(
                        GetTilesPositionsForUpdating().ToList(),
                        Tiles.Select(x => x.Position).ToList(),
                        tileConfigurations,
                        GetCenterTilePosition()
                    )
                );
                RotateTileUnion();
            }
        }

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

        public IEnumerable<string> GetTileMarks(Vector2Int globalPosition)
        {
            return GetTile(globalPosition).Marks;
        }

        private TileImpl GetTile(Vector2Int globalPosition)
        {
            globalPosition -= position;
            return Tiles.FirstOrDefault(x => x.Position == globalPosition);
        }

        private void RotateTileUnion()
        {
            rotation++;
            Vector2 firstCenter = CenterOfMassTools.GetCenterOfMass(
                Tiles.Select(x => x.Position).ToList()
            );
            foreach (TileImpl tile in Tiles)
            {
                tile.SetRotation(tile.Rotation + 1);
                tile.SetPosition(gridProperties, new Vector2Int(tile.Position.y, -tile.Position.x));
            }
            rotation %= 4;
            Vector2 secondCenter = CenterOfMassTools.GetCenterOfMass(
                Tiles.Select(x => x.Position).ToList()
            );
            Vector2 delta = firstCenter - secondCenter;
            foreach (TileImpl tile in Tiles)
            {
                tile.SetPosition(
                    gridProperties,
                    tile.Position + new Vector2Int((int)delta.x, (int)delta.y)
                );
            }
        }

        private IEnumerable<Vector2Int> GetTilesPositionsForUpdating()
        {
            HashSet<Vector2Int> localPositions = new();
            foreach (TileImpl tile in Tiles)
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
            foreach (Vector2Int pos in Tiles.Select(x => x.Position))
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

        public void AddTiles(Dictionary<(Vector2Int globalPosition, int roatation), TileImpl> pairsToAdd)
        {
            foreach (KeyValuePair<(Vector2Int globalPosition, int roatation), TileImpl> pair in pairsToAdd)
            {
                TileImpl newTile = Instantiate(pair.Value, transform);
                Tiles.Add(newTile);
                newTile.SetPosition(gridProperties, pair.Key.globalPosition - position);
                newTile.SetRotation(pair.Key.roatation);
            }
        }
    }
}
