using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TileBuilder;
using TileUnion.Tile;
using UnityEngine;

namespace TileUnion
{
    [Serializable]
    public struct Cost
    {
        [SerializeField]
        private int cost;
        public int Value => cost;
    }

    [Serializable]
    public struct TariffProperties
    {
        [SerializeField]
        private int waterConsumption;
        public int WaterConsumption => waterConsumption;

        [SerializeField]
        private int electricityConsumption;
        public int ElectricityConsumption => electricityConsumption;
    }

    [SelectionBase]
    [AddComponentMenu("Scripts/TileUnion.TileUnion")]
    public partial class TileUnionImpl : MonoBehaviour
    {
        [SerializeField]
        private Cost cost;
        public Cost Cost => cost;

        [SerializeField]
        private TariffProperties tariffProperties;
        public TariffProperties TariffProperties => tariffProperties;

        [Space(20)]
        [SerializeField]
        private Vector2Int position;

        [SerializeField, Range(0, 3)]
        private int rotation;

        [SerializeField]
        private Matrix builderMatrix;

        public Level.Inventory.Room.Model InventoryModel;
        public List<TileImpl> Tiles = new();

        [SerializeField]
        private List<PlaceCondition.SerializedPlaceCondition> serializedPlaceConditions;
        public ImmutableList<PlaceCondition.IPlaceCondition> PlaceConditions =>
            serializedPlaceConditions.Select(x => x.ToPlaceCondition()).ToImmutableList();

        [SerializeField]
        private Dictionary<int, TileUnionConfiguration> cachedUnionConfiguration;

        private class TileUnionConfiguration
        {
            public Vector2Int CenterTilePosition;
            public List<Vector2Int> TilesPositionsForUpdating;
            public List<Vector2Int> TilesPositions;
            public List<TileConfiguration> TilesConfigurations;

            public TileUnionConfiguration(
                List<Vector2Int> tilesPositionsForUpdating,
                List<Vector2Int> tilesPositions,
                List<TileConfiguration> tilesConfigurations,
                Vector2Int centerTilePosition
            )
            {
                TilesPositionsForUpdating = tilesPositionsForUpdating;
                TilesPositions = tilesPositions;
                TilesConfigurations = tilesConfigurations;
                CenterTilePosition = centerTilePosition;
            }
        }

        private class TileConfiguration
        {
            public TileImpl TargetTile;
            public Vector2Int Position;
            public int Rotation;

            public TileConfiguration(TileImpl targetTile, Vector2Int position, int rotation)
            {
                TargetTile = targetTile;
                Position = position;
                Rotation = rotation;
            }
        }

        private Dictionary<int, TileUnionConfiguration> Configuration
        {
            get
            {
                if (cachedUnionConfiguration == null)
                {
                    CreateCache();
                }
                return cachedUnionConfiguration;
            }
        }
        public Vector2Int Position => position;
        public int Rotation => rotation;
        public IEnumerable<Vector2Int> TilesPositionsForUpdating =>
            Configuration[rotation].TilesPositionsForUpdating.Select(x => x + position);
        public IEnumerable<Vector2Int> TilesPositions =>
            Configuration[rotation].TilesPositions.Select(x => x + position);
        public int TilesCount => Tiles.Count;
        public Vector2Int CenterPosition => Configuration[rotation].CenterTilePosition + position;

        public void Move(Direction direction)
        {
            SetPosition(Position + direction.ToVector2Int());
        }

        public bool IsAllWithMark(string mark)
        {
            return Tiles.Select(x => x.Marks.Contains(mark)).All(x => x == true);
        }

        public IEnumerable<Vector2Int> GetImaginePlaces(
            Vector2Int union_position,
            int union_rotation
        )
        {
            return Configuration[union_rotation % 4].TilesPositions.Select(x => x + union_position);
        }

        public Result TryApplyErrorTiles(TileBuilderImpl tile_builder)
        {
            HashSet<TileImpl> invalidTiles = new();
            foreach (TileImpl tile in Tiles)
            {
                Dictionary<Direction, TileImpl> neighbors = new();
                foreach (Direction pos in Direction.Up.GetCircle90())
                {
                    Vector2Int bufferPosition = Position + pos.ToVector2Int() + tile.Position;
                    _ = tile_builder.TileUnionDictionary.TryGetValue(
                        bufferPosition,
                        out TileUnionImpl tileUnion
                    );
                    if (tileUnion != null)
                    {
                        neighbors.Add(
                            pos,
                            tile_builder.TileUnionDictionary[bufferPosition].GetTile(bufferPosition)
                        );
                    }
                    else
                    {
                        neighbors.Add(pos, null);
                    }
                }
                if (tile.RequestWallUpdates(neighbors).Failure)
                {
                    _ = invalidTiles.Add(tile);
                }
                foreach (PlaceCondition.IPlaceCondition condition in PlaceConditions)
                {
                    Result<List<TileImpl>> errorTiles = condition.ApplyCondition(
                        this,
                        tile_builder
                    );
                    if (errorTiles.Success)
                    {
                        foreach (TileImpl errorTile in errorTiles.Data)
                        {
                            _ = invalidTiles.Add(errorTile);
                        }
                    }
                }
            }
            if (invalidTiles.Count > 0)
            {
                foreach (TileImpl tile in invalidTiles)
                {
                    tile.SetTileState(TileImpl.TileState.SelectedAndErrored);
                }
                return new SuccessResult();
            }
            else
            {
                return new FailResult("No error walls");
            }
        }

        public void ApplySelecting()
        {
            foreach (TileImpl tile in Tiles)
            {
                tile.SetTileState(TileImpl.TileState.Selected);
            }
        }

        public void CancelSelecting()
        {
            foreach (TileImpl tile in Tiles)
            {
                tile.SetTileState(TileImpl.TileState.Normal);
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

        public void UpdateWalls(TileBuilderImpl tile_builder, Vector2Int position)
        {
            TileImpl tile = GetTile(position);
            Dictionary<Direction, TileImpl> neighbors = new();
            foreach (Direction pos in Direction.Up.GetCircle90())
            {
                Vector2Int bufferPosition = position + pos.ToVector2Int();
                _ = tile_builder.TileUnionDictionary.TryGetValue(
                    bufferPosition,
                    out TileUnionImpl tileUnion
                );
                if (tileUnion != null)
                {
                    neighbors.Add(
                        pos,
                        tile_builder.TileUnionDictionary[bufferPosition].GetTile(bufferPosition)
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

        public void UpdateCorners(TileBuilderImpl tile_builder, Vector2Int position)
        {
            TileImpl tile = GetTile(position);
            Dictionary<Direction, TileImpl> neighbors = new();
            foreach (Direction pos in Direction.Up.GetCircle45())
            {
                Vector2Int bufferPosition = position + pos.ToVector2Int();
                _ = tile_builder.TileUnionDictionary.TryGetValue(
                    bufferPosition,
                    out TileUnionImpl tileUnion
                );
                if (tileUnion != null)
                {
                    neighbors.Add(
                        pos,
                        tile_builder.TileUnionDictionary[bufferPosition].GetTile(bufferPosition)
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
            cachedUnionConfiguration = new();
            for (int i = 0; i < 4; i++)
            {
                List<TileConfiguration> tileConfigurations = new();
                foreach (TileImpl tile in Tiles)
                {
                    tileConfigurations.Add(new(tile, tile.Position, tile.Rotation));
                }
                cachedUnionConfiguration.Add(
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
            this.rotation = rotation < 0 ? (rotation % 4) + 4 : rotation % 4;
            foreach (TileConfiguration config in Configuration[this.rotation].TilesConfigurations)
            {
                config.TargetTile.SetPosition(config.Position);
                config.TargetTile.SetRotation(config.Rotation);
            }
        }

        public void SetPosition(Vector2Int vector)
        {
            position = vector;
            transform.localPosition = new Vector3(
                builderMatrix.Step * position.y,
                transform.localPosition.y,
                -builderMatrix.Step * position.x
            );
        }

        public IEnumerable<string> GetTileMarks(Vector2Int global_position)
        {
            return GetTile(global_position).Marks;
        }

        private TileImpl GetTile(Vector2Int global_position)
        {
            global_position -= position;
            return Tiles.FirstOrDefault(x => x.Position == global_position);
        }

        private void RotateTileUnion()
        {
            rotation++;
            Vector2 first_center = CenterOfMassTools.GetCenterOfMass(
                Tiles.Select(x => x.Position).ToList()
            );
            foreach (TileImpl tile in Tiles)
            {
                tile.SetRotation(tile.Rotation + 1);
                tile.SetPosition(new Vector2Int(tile.Position.y, -tile.Position.x));
            }
            rotation %= 4;
            Vector2 second_center = CenterOfMassTools.GetCenterOfMass(
                Tiles.Select(x => x.Position).ToList()
            );
            Vector2 delta = first_center - second_center;
            foreach (TileImpl tile in Tiles)
            {
                tile.SetPosition(tile.Position + new Vector2Int((int)delta.x, (int)delta.y));
            }
        }

        private IEnumerable<Vector2Int> GetTilesPositionsForUpdating()
        {
            HashSet<Vector2Int> local_positions = new();
            foreach (TileImpl tile in Tiles)
            {
                foreach (Direction position in Direction.Up.GetCircle45())
                {
                    Vector2Int pos = tile.Position + position.ToVector2Int();
                    _ = local_positions.Add(pos);
                }
                _ = local_positions.Add(tile.Position);
            }
            return local_positions;
        }

        private Vector2Int GetCenterTilePosition()
        {
            Vector2 VectorSum = new();
            foreach (Vector2Int pos in Tiles.Select(x => x.Position))
            {
                VectorSum += pos;
            }
            VectorSum /= TilesCount;
            List<Vector2Int> vectors =
                new()
                {
                    new((int)Math.Truncate(VectorSum.x), (int)Math.Truncate(VectorSum.y)),
                    new(
                        (int)Math.Truncate(VectorSum.x),
                        (int)Math.Truncate(VectorSum.y) + (int)VectorSum.normalized.y
                    ),
                    new(
                        (int)Math.Truncate(VectorSum.x) + (int)VectorSum.normalized.x,
                        (int)Math.Truncate(VectorSum.y)
                    ),
                    new(
                        (int)Math.Truncate(VectorSum.x) + (int)VectorSum.normalized.x,
                        (int)Math.Truncate(VectorSum.y) + (int)VectorSum.normalized.y
                    )
                };
            return vectors.OrderBy(x => Vector2.Distance(x, VectorSum)).First();
        }
    }
}
