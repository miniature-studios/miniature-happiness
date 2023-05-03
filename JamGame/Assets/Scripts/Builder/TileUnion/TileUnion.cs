using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SelectionBase]
public class TileUnion : MonoBehaviour
{
    [SerializeField] public GameObject UIPrefab;
    [SerializeField] public List<Tile> tiles = new();
    [SerializeField] Vector2Int position;
    [SerializeField] int rotation;
    [SerializeField] BuilderMatrix builderMatrix;
    
    class TileUnionConfiguration
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
    class TileConfiguration
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
    Dictionary<int, TileUnionConfiguration> cachedUnionConfiguration = null;

    Dictionary<int, TileUnionConfiguration> CachedUnionConfiguration
    {
        get
        {
            if (cachedUnionConfiguration == null)
            {
                CreateTileUnionCehe();
            }
            return cachedUnionConfiguration;
        }
    }
    public Vector2Int Position { get => position; set => SetTileUnionPosition(value); }
    public int Rotation { get => rotation; set => SetTileUnionRotationFromCehe(value); }
    public IEnumerable<Vector2Int> TilesPositionsForUpdating { get => CachedUnionConfiguration[rotation].TilesPositionsForUpdating.Select(x => x + position); }
    public IEnumerable<Vector2Int> TilesPositions { get => CachedUnionConfiguration[rotation].TilesPositions.Select(x => x + position); }
    public int TilesCount { get => tiles.Count; }
    public Vector2Int CenterTilePosition { get => CachedUnionConfiguration[rotation].CenterTilePosition + position; }

    public void Move(Direction direction)
    {
        Position += direction.ToVector2Int();
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
        foreach (var tile in tiles)
        {
            Dictionary<Direction, Tile> neighbours = new();
            foreach (var pos in Direction.Up.GetCircle90())
            {
                var bufferPosition = Position + pos.ToVector2Int() + tile.Position;
                tileBuilder.TileUnionDictionary.TryGetValue(bufferPosition, out TileUnion tileUnion);
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
            foreach (var tile in invalidTiles)
            {
                tile.ApplyErrorState();
            }
            return new SuccessResult();
        }
        else
        {
            return new FailResult("No error walls");
        }
    }
    public void CancelErrorTiles()
    {
        foreach (var tile in tiles)
        {
            tile.CancelErrorState();
        }
    }
    public void ApplySelecting()
    {
        foreach (var tile in tiles)
        {
            tile.ApplySelectState();
        }
    }
    public void CancelSelecting()
    {
        foreach (var tile in tiles)
        {
            tile.CancelSelectState();
        }
    }
    public void UpdateWallsInTile(TileBuilder tileBuilder, Vector2Int position)
    {
        var tile = GetTile(position);
        Dictionary<Direction, Tile> neighbours = new();
        foreach (var pos in Direction.Up.GetCircle90())
        {
            var bufferPosition = position + pos.ToVector2Int();
            tileBuilder.TileUnionDictionary.TryGetValue(bufferPosition, out TileUnion tileUnion);
            if(tileUnion != null)
            {
                neighbours.Add(pos, tileBuilder.TileUnionDictionary[bufferPosition].GetTile(bufferPosition));
            }
            else
            {
                neighbours.Add(pos, null);
            }
        }
        var result = tile.RequestWallUpdates(neighbours);
        if (result.Success)
        {
            tile.ApplyUpdatingWalls(result as SuccessResult<Dictionary<Direction, TileWallType>>);
        }
    }
    public void UpdateCornersInTile(TileBuilder tileBuilder, Vector2Int position)
    {
        var tile = GetTile(position);
        Dictionary<Direction, Tile> neighbours = new();
        foreach (var pos in Direction.Up.GetCircle45())
        {
            var bufferPosition = position + pos.ToVector2Int();
            tileBuilder.TileUnionDictionary.TryGetValue(bufferPosition, out TileUnion tileUnion);
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
        foreach (var tile in tiles)
        {
            Dictionary<Direction, Tile> neighbours = new();
            foreach (var pos in Direction.Up.GetCircle90())
            {
                var bufferPosition = tile.Position + pos.ToVector2Int();
                if (tiles.Select(x => x.Position).Contains(bufferPosition))
                {
                    neighbours.Add(pos, tiles.Find(x => x.Position == bufferPosition));
                }
                else
                {
                    neighbours.Add(pos, null);
                }
            }
            var result = tile.RequestWallUpdates(neighbours);
            if (result.Success)
            {
                tile.ApplyUpdatingWalls(result as SuccessResult<Dictionary<Direction, TileWallType>>);
            }
        }
    }

    public void CreateTileUnionCehe()
    {
        cachedUnionConfiguration = new();
        for (int i = 0; i < 4; i++)
        {
            List<TileConfiguration> tileConfigurations = new();
            foreach (var tile in tiles)
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

    Tile GetTile(Vector2Int plobalPosition)
    {
        plobalPosition -= position;
        return tiles.FirstOrDefault(x => x.Position == plobalPosition);
    }
    void RotateTileUnion()
    {
        rotation++;
        var firstCenter = TileUnionTools.GetCenterOfMass(tiles.Select(x => x.Position).ToList());
        foreach (var tile in tiles)
        {
            tile.Rotation++;
            tile.Position = new Vector2Int(tile.Position.y, -tile.Position.x);
        }
        rotation %= 4;
        var secondCenter = TileUnionTools.GetCenterOfMass(tiles.Select(x => x.Position).ToList());
        var delta = (firstCenter - secondCenter);
        foreach (var tile in tiles)
        {
            tile.Position += new Vector2Int((int)delta.x, (int)delta.y);
        }
    }
    void SetTileUnionRotationFromCehe(int rotation)
    {
        this.rotation = rotation % 4;
        foreach (var config in CachedUnionConfiguration[this.rotation].TilesConfigurations)
        {
            config.TargetTile.Position = config.Position;
            config.TargetTile.Rotation = config.Rotation;
        }
    }
    IEnumerable<Vector2Int> GetTilesPositionsForUpdating()
    {
        HashSet<Vector2Int> localPositions = new();
        foreach (var tile in tiles)
        {
            foreach (var position in Direction.Up.GetCircle45())
            {
                var pos = tile.Position + position.ToVector2Int();
                localPositions.Add(pos);
            }
            localPositions.Add(tile.Position);
        }
        return localPositions;
    }
    void SetTileUnionPosition(Vector2Int vector)
    {
        position = vector;
        transform.localPosition = new Vector3(
            builderMatrix.Step * position.y,
            transform.localPosition.y,
            -builderMatrix.Step * position.x
            );
    }
    Vector2Int GetCenterTilePosition()
    {
        Vector2 VectorSum = new();
        foreach (var pos in tiles.Select(x => x.Position))
        {
            VectorSum += pos;
        }
        VectorSum /= TilesCount;
        List<Vector2Int> vectors = new();
        vectors.Add(new((int)Math.Truncate(VectorSum.x), (int)Math.Truncate(VectorSum.y)));
        vectors.Add(new((int)Math.Truncate(VectorSum.x), (int)Math.Truncate(VectorSum.y) + (int)VectorSum.normalized.y));
        vectors.Add(new((int)Math.Truncate(VectorSum.x) + (int)VectorSum.normalized.x, (int)Math.Truncate(VectorSum.y)));
        vectors.Add(new((int)Math.Truncate(VectorSum.x) + (int)VectorSum.normalized.x, (int)Math.Truncate(VectorSum.y) + (int)VectorSum.normalized.y));
        return vectors.OrderBy(x => Vector2.Distance(x, VectorSum)).First();
    }
}