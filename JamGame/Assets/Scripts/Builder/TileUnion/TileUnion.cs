using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
[RequireComponent(typeof(RoomProperties))]
public class TileUnion : MonoBehaviour
{
    [SerializeField] public GameObject UIPrefab;
    [SerializeField] public List<Tile> tiles = new();
    [SerializeField] private Vector2Int position;
    [SerializeField, Range(0, 3)] private int rotation;
    [SerializeField] private BuilderMatrix builderMatrix;
    private Dictionary<int, TileUnionConfiguration> cachedUnionConfiguration = null;

    private class TileUnionConfiguration
    {
        public Vector2Int CenterTilePosition;
        public List<Vector2Int> TilesPositionsForUpdating;
        public List<Vector2Int> TilesPositions;
        public List<TileConfiguration> TilesConfigurations;
        public TileUnionConfiguration(List<Vector2Int> tilesPositionsForUpdating, List<Vector2Int> tilesPositions, List<TileConfiguration> tilesConfigurations, Vector2Int centerTilePosition)
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

    private Dictionary<int, TileUnionConfiguration> CachedUnionConfiguration
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
    public IEnumerable<Vector2Int> TilesPositionsForUpdating => CachedUnionConfiguration[rotation].TilesPositionsForUpdating.Select(x => x + position);
    public IEnumerable<Vector2Int> TilesPositions => CachedUnionConfiguration[rotation].TilesPositions.Select(x => x + position);
    public int TilesCount => tiles.Count;
    public Vector2Int CenterPosition => CachedUnionConfiguration[rotation].CenterTilePosition + position;

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
        return tiles.Select(x => x.Marks.Contains(mark)).All(x => x == true);
    }
    public IEnumerable<Vector2Int> GetImaginePlaces(Vector2Int unionPosition, int unionRotation)
    {
        return CachedUnionConfiguration[unionRotation % 4].TilesPositions.Select(x => x + unionPosition);
    }
    public Result TryApplyErrorTiles(TileBuilder tileBuilder)
    {
        List<Tile> invalidTiles = new();
        foreach (Tile tile in tiles)
        {
            Dictionary<Direction, Tile> neighbours = new();
            foreach (Direction pos in Direction.Up.GetCircle90())
            {
                Vector2Int bufferPosition = Position + pos.ToVector2Int() + tile.Position;
                _ = tileBuilder.TileUnionDictionary.TryGetValue(bufferPosition, out TileUnion tileUnion);
                if (tileUnion != null)
                {
                    neighbours.Add(pos, tileBuilder.TileUnionDictionary[bufferPosition].GetTile(bufferPosition));
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
        foreach (Tile tile in tiles)
        {
            tile.SetTileState(Tile.TileState.Selected);
        }
    }
    public void CancelSelecting()
    {
        foreach (Tile tile in tiles)
        {
            tile.SetTileState(Tile.TileState.Normal);
        }
    }
    public void UpdateWalls(TileBuilder tileBuilder, Vector2Int position)
    {
        Tile tile = GetTile(position);
        Dictionary<Direction, Tile> neighbours = new();
        foreach (Direction pos in Direction.Up.GetCircle90())
        {
            Vector2Int bufferPosition = position + pos.ToVector2Int();
            _ = tileBuilder.TileUnionDictionary.TryGetValue(bufferPosition, out TileUnion tileUnion);
            if (tileUnion != null)
            {
                neighbours.Add(pos, tileBuilder.TileUnionDictionary[bufferPosition].GetTile(bufferPosition));
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
    public void UpdateCorners(TileBuilder tileBuilder, Vector2Int position)
    {
        Tile tile = GetTile(position);
        Dictionary<Direction, Tile> neighbours = new();
        foreach (Direction pos in Direction.Up.GetCircle45())
        {
            Vector2Int bufferPosition = position + pos.ToVector2Int();
            _ = tileBuilder.TileUnionDictionary.TryGetValue(bufferPosition, out TileUnion tileUnion);
            if (tileUnion != null)
            {
                neighbours.Add(pos, tileBuilder.TileUnionDictionary[bufferPosition].GetTile(bufferPosition));
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
        foreach (Tile tile in tiles)
        {
            Dictionary<Direction, Tile> neighbours = new();
            foreach (Direction pos in Direction.Up.GetCircle90())
            {
                Vector2Int bufferPosition = tile.Position + pos.ToVector2Int();
                if (tiles.Select(x => x.Position).Contains(bufferPosition))
                {
                    neighbours.Add(pos, tiles.Find(x => x.Position == bufferPosition));
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

    private void CreateCache()
    {
        cachedUnionConfiguration = new();
        for (int i = 0; i < 4; i++)
        {
            List<TileConfiguration> tileConfigurations = new();
            foreach (Tile tile in tiles)
            {
                tileConfigurations.Add(new(tile, tile.Position, tile.Rotation));
            }
            cachedUnionConfiguration.Add(rotation,
                new(GetTilesPositionsForUpdating().ToList(),
                    tiles.Select(x => x.Position).ToList(),
                    tileConfigurations,
                    GetCenterTilePosition())
                );
            RotateTileUnion();
        }
    }

    public void SetRotation(int rotation)
    {
        this.rotation = rotation < 0 ? (rotation % 4) + 4 : rotation % 4;
        foreach (TileConfiguration config in CachedUnionConfiguration[this.rotation].TilesConfigurations)
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

    private Tile GetTile(Vector2Int plobalPosition)
    {
        plobalPosition -= position;
        return tiles.FirstOrDefault(x => x.Position == plobalPosition);
    }

    private void RotateTileUnion()
    {
        rotation++;
        Vector2 firstCenter = TileUnionTools.GetCenterOfMass(tiles.Select(x => x.Position).ToList());
        foreach (Tile tile in tiles)
        {
            tile.SetRotation(tile.Rotation + 1);
            tile.SetPosition(new Vector2Int(tile.Position.y, -tile.Position.x));
        }
        rotation %= 4;
        Vector2 secondCenter = TileUnionTools.GetCenterOfMass(tiles.Select(x => x.Position).ToList());
        Vector2 delta = firstCenter - secondCenter;
        foreach (Tile tile in tiles)
        {
            tile.SetPosition(tile.Position + new Vector2Int((int)delta.x, (int)delta.y));
        }
    }

    private IEnumerable<Vector2Int> GetTilesPositionsForUpdating()
    {
        HashSet<Vector2Int> localPositions = new();
        foreach (Tile tile in tiles)
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
        Vector2 VectorSum = new();
        foreach (Vector2Int pos in tiles.Select(x => x.Position))
        {
            VectorSum += pos;
        }
        VectorSum /= TilesCount;
        List<Vector2Int> vectors = new()
        {
            new((int)Math.Truncate(VectorSum.x), (int)Math.Truncate(VectorSum.y)),
            new((int)Math.Truncate(VectorSum.x), (int)Math.Truncate(VectorSum.y) + (int)VectorSum.normalized.y),
            new((int)Math.Truncate(VectorSum.x) + (int)VectorSum.normalized.x, (int)Math.Truncate(VectorSum.y)),
            new((int)Math.Truncate(VectorSum.x) + (int)VectorSum.normalized.x, (int)Math.Truncate(VectorSum.y) + (int)VectorSum.normalized.y)
        };
        return vectors.OrderBy(x => Vector2.Distance(x, VectorSum)).First();
    }
}