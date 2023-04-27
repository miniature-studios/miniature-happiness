using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileUnion : MonoBehaviour
{
    [SerializeField] public List<Tile> tiles = new();
    [SerializeField] Vector2Int unionPosition;
    [SerializeField] int unionRotation;
    float step = 5;
    public Vector2Int Position
    {
        get { return unionPosition; }
        set {
            unionPosition = value;
            transform.localPosition = new Vector3(
                step * unionPosition.y,
                transform.localPosition.y,
                -step * unionPosition.x
                );
        }
    }
    public int Rotation
    {
        get { return unionRotation; }
        set
        {
            while (value < 0)
                value += 4;
            while (value > 3)
                value -= 4;
            while (unionRotation != value)
                RotateRight();
        }
    }
    public List<Vector2Int> TilesPositionsForUpdating
    {
        get 
        {
            HashSet<Vector2Int> buffer = new();
            foreach (var tile in tiles)
            {
                for (int i = 1; i >= -1; i--)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        var pos = new Vector2Int(tile.Position.x + j, tile.Position.y + i);
                        buffer.Add(pos);
                    }
                }
            }
            return buffer.Select(arg => new Vector2Int(arg.x + Position.x, arg.y + Position.y)).ToList();
        }
    }
    public List<Vector2Int> TilesPositions
    {
        get { return tiles.Select(x => x.Position + Position).ToList(); }
    }
    public Vector3 TileUnionCenter
    {
        get
        {
            Vector3 vector = new();
            foreach (var tile in tiles)
            {
                vector += tile.transform.position;
            }
            vector /= tiles.Count;
            return vector;
        }
    }
    public int TileCount { get { return tiles.Count; } }
    public void Move(Direction direction)
    {
        Position += direction.ToVector2Int();
    }
    public Tile GetTile(Vector2Int position)
    {
        position -= Position;
        return tiles.Find(x => x.Position == position);
    }
    public bool IsAllWithMark(string mark)
    {
        return tiles.Select(x => x.Marks.Contains(mark)).All(x => x == true);
    }
    public bool IsContainsTile(Tile tile)
    {
        return tiles.Contains(tile);
    }
    public List<Vector2Int> GetImaginePlaces(Vector2Int unionPosition, int unionRotation)
    {
        var list = tiles.Select(x => x.Position).ToList();
        // Rotating
        for (int j = Rotation; j < unionRotation; j++)
        {
            var firstCenter = GetCenterMass(list);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = new(list[i].y, -list[i].x);
            }
            var secondCenter = GetCenterMass(list);
            var delta = (firstCenter - secondCenter);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] += new Vector2Int((int)delta.x, (int)delta.y);
            }
        }
        return list.Select(x => x + unionPosition).ToList();
    }
    public List<Tile> IsValidPlacing(Dictionary<Vector2Int, Tile> tilePairs)
    {
        Dictionary<Vector2Int, Tile> localTilePairs = new();
        foreach (var pairs in tilePairs)
        {
            localTilePairs.Add(pairs.Key - Position, pairs.Value);
        }
        List<Tile> invalidTiles = new();
        foreach (var tile in tiles)
        {
            List<Tile> tilesAround = new();
            for (int i = 1; i >= -1; i--)
            {
                for (int j = -1; j <= 1; j++)
                {
                    tilesAround.Add(localTilePairs[tile.Position + new Vector2Int(j, i)]);
                }
            }
            if (!tile.TryUpdateWalls(tilesAround))
                invalidTiles.Add(tile);
        }
        return invalidTiles;
    }
    public void UpdateWalls(Dictionary<Vector2Int, Tile> tilePairs)
    {
        Dictionary<Vector2Int, Tile> localTilePairs = new();
        foreach (var pairs in tilePairs)
        {
            localTilePairs.Add(pairs.Key - Position, pairs.Value);
        }
        foreach (var tile in tiles)
        {
            List<Tile> tilesAround = new();
            for (int i = 1; i >= -1; i--)
            {
                for (int j = -1; j <= 1; j++)
                {
                    tilesAround.Add(localTilePairs[tile.Position + new Vector2Int(j, i)]);
                }
            }
            tile.UpdateWalls(tilesAround);
        }
    }
    public void UpdateCorners(Dictionary<Vector2Int, Tile> tilePairs)
    {
        Dictionary<Vector2Int, Tile> localTilePairs = new();
        foreach (var pairs in tilePairs)
        {
            localTilePairs.Add(pairs.Key - Position, pairs.Value);
        }
        foreach (var tile in tiles)
        {
            List<Tile> tilesAround = new();
            for (int i = 1; i >= -1; i--)
            {
                for (int j = -1; j <= 1; j++)
                {
                    tilesAround.Add(localTilePairs[tile.Position + new Vector2Int(j, i)]);
                }
            }
            tile.UpdateCorners(tilesAround);
        }
    }
    void RotateRight()
    {
        unionRotation++;
        var firstCenter = GetCenterMass(tiles.Select(x => x.Position).ToList());
        foreach (var tile in tiles)
        {
            tile.Rotation++;
            tile.Position = new Vector2Int(tile.Position.y, -tile.Position.x);
        } 
        unionRotation %= 4;
        var secondCenter = GetCenterMass(tiles.Select(x => x.Position).ToList());
        var delta = (firstCenter - secondCenter);
        foreach (var tile in tiles)
        {
            tile.Position += new Vector2Int((int)delta.x, (int)delta.y);
        }
    }
    Vector2 GetCenterMass(List<Vector2Int> positions)
    {
        Vector2 firstPos = new();
        foreach (var pos in positions)
        {
            firstPos += pos;
        }
        firstPos /= positions.Count;
        return GetRightPoint(firstPos);
    }
    Vector2 GetRightPoint(Vector2 vector)
    {
        if((Math.Abs(vector.x % 1) == 0.5 && Math.Abs(vector.y % 1) == 0.5)
            || (vector.x % 1 == 0 && vector.y % 1 == 0))
        {
            return vector;
        }
        List<Vector2> variants = new();
        if (Math.Abs(vector.x % 1) == 0.5)
        {
            float tail = Math.Abs(vector.y % 1);
            if (tail < 0.5)
            {
                variants.Add(new(vector.x, (float)Math.Truncate(vector.y)));
                variants.Add(new(vector.x, (float)Math.Truncate(vector.y) + 0.5f * vector.normalized.y));
            }
            else
            {
                variants.Add(new(vector.x, (float)Math.Truncate(vector.y) + 0.5f * vector.normalized.y));
                variants.Add(new(vector.x, (float)Math.Truncate(vector.y) + vector.normalized.y));
            }
        }
        else if (Math.Abs(vector.y % 1) == 0.5)
        {
            float tail = Math.Abs(vector.x % 1);
            if (tail < 0.5)
            {
                variants.Add(new((float)Math.Truncate(vector.x), vector.y));
                variants.Add(new((float)Math.Truncate(vector.x) + 0.5f * vector.normalized.x, vector.y));
            }
            else
            {
                variants.Add(new((float)Math.Truncate(vector.x) + 0.5f * vector.normalized.x, vector.y));
                variants.Add(new((float)Math.Truncate(vector.x) + vector.normalized.x, vector.y));
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                variants.Add(new());
            }
            float tail_x = Math.Abs(vector.x % 1);
            float tail_y = Math.Abs(vector.y % 1);
            if(tail_x < 0.5)
            {
                variants[0] = new Vector2((float)Math.Truncate(vector.x), variants[0].y);
                variants[1] = new Vector2((float)Math.Truncate(vector.x) + 0.5f * vector.normalized.x, variants[1].y);
                variants[2] = new Vector2((float)Math.Truncate(vector.x), variants[2].y);
                variants[3] = new Vector2((float)Math.Truncate(vector.x) + 0.5f * vector.normalized.x, variants[3].y);
            }
            else
            {
                variants[0] = new Vector2((float)Math.Truncate(vector.x) + vector.normalized.x, variants[0].y);
                variants[1] = new Vector2((float)Math.Truncate(vector.x) + 0.5f * vector.normalized.x, variants[1].y);
                variants[2] = new Vector2((float)Math.Truncate(vector.x) + vector.normalized.x, variants[2].y);
                variants[3] = new Vector2((float)Math.Truncate(vector.x) + 0.5f * vector.normalized.x, variants[3].y);
            }
            if (tail_y < 0.5)
            {
                variants[0] = new Vector2(variants[0].x, (float)Math.Truncate(vector.y));
                variants[1] = new Vector2(variants[1].x, (float)Math.Truncate(vector.y));
                variants[2] = new Vector2(variants[2].x, (float)Math.Truncate(vector.y) + 0.5f * vector.normalized.y);
                variants[3] = new Vector2(variants[3].x, (float)Math.Truncate(vector.y) + 0.5f * vector.normalized.y);
            }
            else
            {
                variants[0] = new Vector2(variants[0].x, (float)Math.Truncate(vector.y) + 0.5f * vector.normalized.y);
                variants[1] = new Vector2(variants[1].x, (float)Math.Truncate(vector.y) + 0.5f * vector.normalized.y);
                variants[2] = new Vector2(variants[2].x, (float)Math.Truncate(vector.y) + vector.normalized.y);
                variants[3] = new Vector2(variants[3].x, (float)Math.Truncate(vector.y) + vector.normalized.y);
            }
        }
        return variants.OrderBy(x => Vector2.Distance(x, vector)).First();
    }
}