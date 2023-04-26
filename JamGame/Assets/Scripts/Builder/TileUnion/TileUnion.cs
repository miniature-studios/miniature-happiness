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
    public (Dictionary<Vector2Int, Tile>, List<Vector2Int>) GetImagineUpdatingPlaces(Vector2Int unionPosition, int unionRotation)
    {
        Dictionary<Vector2Int, Tile> dict = new();

        foreach (var tile in tiles)
        {
            dict.Add(tile.Position, tile);
        }

        Dictionary<Vector2Int, Tile> newDict = new();

        // Rotating
        for (int j = Rotation; j < unionRotation; j++)
        {
            var firstCenter = GetCenterMass(dict.Select(x => x.Key).ToList());
            foreach (var item in dict)
            {
                newDict.Add(new(item.Key.y, -item.Key.x), item.Value);
            }
            var secondCenter = GetCenterMass(dict.Select(x => x.Key).ToList());
            var delta = (firstCenter - secondCenter);
            dict.Clear();
            foreach (var item in newDict)
            {
                dict.Add(new(item.Key.x + (int)delta.x, item.Key.y + (int)delta.y), item.Value);
            }
        }

        HashSet<Vector2Int> buffer = new();
        foreach (var place in dict.Keys)
        {
            for (int i = 1; i >= -1; i--)
            {
                for (int j = -1; j <= 1; j++)
                {
                    var pos = new Vector2Int(place.x + j, place.y + i);
                    buffer.Add(pos);
                }
            }
        }
        List<Vector2Int> nullPositions = new();
        foreach (var position in buffer)
        {
            if (!dict.ContainsKey(position))
                nullPositions.Add(position + unionPosition);
        }
        newDict.Clear();
        foreach (var item in dict)
        {
            newDict.Add(new Vector2Int(item.Key.x + unionPosition.x, item.Key.y + unionPosition.y), item.Value);
        }
        return (newDict, nullPositions);
    }
    public List<Tile> IsValidPlacing(Dictionary<Vector2Int, Tile> tilePairs, Vector2Int unionPosition, int unionRotation)
    {
        List<Tile> invalidTiles = new();
        var ImPlaces = GetImaginePlaces(unionPosition, unionRotation);
        foreach (var place in ImPlaces)
        {
            List<Tile> tilesAround = new();
            for (int i = 1; i >= -1; i--)
            {
                for (int j = -1; j <= 1; j++)
                {
                    tilesAround.Add(tilePairs[place + new Vector2Int(j, i)]);
                }
            }
            if (!tilePairs[place].TryUpdateWalls(tilesAround))
                invalidTiles.Add(tilePairs[place]);
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
        if (Math.Abs(vector.x % 1) == 0.5)
        {
            // on x right axis TODO
        }
        if (Math.Abs(vector.y % 1) == 0.5)
        {
            // on y right axis TODO
        }
        /* TODO
        List<Vector2> list = new();
        list.Add(new((float)Math.Truncate(vector.x), (float)Math.Truncate(vector.y)));
        list.Add(new(list.First().x + vector.normalized.x, list.First().y));
        list.Add(new(list.First().x, list.First().y + vector.normalized.y));
        list.Add(new(list.First().x + vector.normalized.x, list.First().y + vector.normalized.y));
        list.Add(new(list.First().x + vector.normalized.x/2, list.First().y + vector.normalized.y/2));
        list = list.Select(x => x = new Vector2(x.x + vector.normalized.x/2, x.y + vector.normalized.y / 2)).ToList();
        return list.OrderBy(x => Vector2.Distance(x, vector)).First();
        */
        throw new NotImplementedException();
    }
}