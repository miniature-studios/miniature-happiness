using Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TileUnion
{
    [SelectionBase]
    [RequireComponent(typeof(RoomProperties))]
    [RequireComponent(typeof(RoomViewProperties))]
    public class TileUnion : MonoBehaviour
    {
        [SerializeField]
        private Vector2Int position;

        [SerializeField, Range(0, 3)]
        private int rotation;

        [SerializeField]
        private BuilderMatrix builderMatrix;

        public RoomInventoryUI UIPrefab;
        public List<Tile> Tiles = new();

        private Dictionary<int, TileUnionConfiguration> cachedUnionConfiguration = null;

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
            public Tile TargetTile;
            public Vector2Int Position;
            public int Rotation;

            public TileConfiguration(Tile targetTile, Vector2Int position, int rotation)
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

        private void OnValidate()
        {
            CreateCache();
            SetPosition(Position);
            SetRotation(Rotation);
        }

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

        public Result TryApplyErrorTiles(TileBuilder tile_builder)
        {
            List<Tile> invalidTiles = new();
            foreach (Tile tile in Tiles)
            {
                Dictionary<Direction, Tile> neighbours = new();
                foreach (Direction pos in Direction.Up.GetCircle90())
                {
                    Vector2Int bufferPosition = Position + pos.ToVector2Int() + tile.Position;
                    _ = tile_builder.TileUnionDictionary.TryGetValue(
                        bufferPosition,
                        out TileUnion tileUnion
                    );
                    if (tileUnion != null)
                    {
                        neighbours.Add(
                            pos,
                            tile_builder.TileUnionDictionary[bufferPosition].GetTile(bufferPosition)
                        );
                    }
                    else
                    {
                        neighbours.Add(pos, null);
                    }
                }
                if (tile.RequestWallUpdates(neighbours).Failure)
                {
                    invalidTiles.Add(tile);
                }
            }
            if (invalidTiles.Count > 0)
            {
                foreach (Tile tile in invalidTiles)
                {
                    tile.SetTileState(Tile.TileState.SelectedAndErrored);
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
            foreach (Tile tile in Tiles)
            {
                tile.SetTileState(Tile.TileState.Selected);
            }
        }

        public void CancelSelecting()
        {
            foreach (Tile tile in Tiles)
            {
                tile.SetTileState(Tile.TileState.Normal);
            }
        }

        public IEnumerable<Direction> GetAccessibleDirectionsFromPosition(Vector2Int position)
        {
            return Tiles.Find(x => x.Position == position - this.position).GetPassableDirections();
        }

        public void ShowInvalidPlacing()
        {
            foreach (Tile tile in Tiles)
            {
                tile.SetTileState(Tile.TileState.SelectedAndErrored);
            }
            _ = StartCoroutine(ShowInvalidPlacingRoutine());
        }

        private IEnumerator ShowInvalidPlacingRoutine()
        {
            yield return new WaitForSeconds(1);
            foreach (Tile tile in Tiles)
            {
                tile.SetTileState(Tile.TileState.Normal);
            }
        }

        public void UpdateWalls(TileBuilder tile_builder, Vector2Int position)
        {
            Tile tile = GetTile(position);
            Dictionary<Direction, Tile> neighbours = new();
            foreach (Direction pos in Direction.Up.GetCircle90())
            {
                Vector2Int bufferPosition = position + pos.ToVector2Int();
                _ = tile_builder.TileUnionDictionary.TryGetValue(
                    bufferPosition,
                    out TileUnion tileUnion
                );
                if (tileUnion != null)
                {
                    neighbours.Add(
                        pos,
                        tile_builder.TileUnionDictionary[bufferPosition].GetTile(bufferPosition)
                    );
                }
                else
                {
                    neighbours.Add(pos, null);
                }
            }
            Result<Tile.WallTypeMatch> result = tile.RequestWallUpdates(neighbours);
            if (result.Success)
            {
                tile.ApplyUpdatingWalls(result);
            }
        }

        public void UpdateCorners(TileBuilder tile_builder, Vector2Int position)
        {
            Tile tile = GetTile(position);
            Dictionary<Direction, Tile> neighbours = new();
            foreach (Direction pos in Direction.Up.GetCircle45())
            {
                Vector2Int bufferPosition = position + pos.ToVector2Int();
                _ = tile_builder.TileUnionDictionary.TryGetValue(
                    bufferPosition,
                    out TileUnion tileUnion
                );
                if (tileUnion != null)
                {
                    neighbours.Add(
                        pos,
                        tile_builder.TileUnionDictionary[bufferPosition].GetTile(bufferPosition)
                    );
                }
                else
                {
                    neighbours.Add(pos, null);
                }
            }
            tile.UpdateCorners(neighbours);
        }

        public void IsolateUpdate()
        {
            foreach (Tile tile in Tiles)
            {
                Dictionary<Direction, Tile> neighbours = new();
                foreach (Direction pos in Direction.Up.GetCircle90())
                {
                    Vector2Int bufferPosition = tile.Position + pos.ToVector2Int();
                    if (Tiles.Select(x => x.Position).Contains(bufferPosition))
                    {
                        neighbours.Add(
                            pos,
                            Tiles.FirstOrDefault(x => x.Position == bufferPosition)
                        );
                    }
                    else
                    {
                        neighbours.Add(pos, null);
                    }
                }
                Result<Tile.WallTypeMatch> result = tile.RequestWallUpdates(neighbours);
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
                foreach (Tile tile in Tiles)
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

        private Tile GetTile(Vector2Int global_position)
        {
            global_position -= position;
            return Tiles.FirstOrDefault(x => x.Position == global_position);
        }

        private void RotateTileUnion()
        {
            rotation++;
            Vector2 first_center = TileUnionTools.GetCenterOfMass(
                Tiles.Select(x => x.Position).ToList()
            );
            foreach (Tile tile in Tiles)
            {
                tile.SetRotation(tile.Rotation + 1);
                tile.SetPosition(new Vector2Int(tile.Position.y, -tile.Position.x));
            }
            rotation %= 4;
            Vector2 second_center = TileUnionTools.GetCenterOfMass(
                Tiles.Select(x => x.Position).ToList()
            );
            Vector2 delta = first_center - second_center;
            foreach (Tile tile in Tiles)
            {
                tile.SetPosition(tile.Position + new Vector2Int((int)delta.x, (int)delta.y));
            }
        }

        private IEnumerable<Vector2Int> GetTilesPositionsForUpdating()
        {
            HashSet<Vector2Int> local_positions = new();
            foreach (Tile tile in Tiles)
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
