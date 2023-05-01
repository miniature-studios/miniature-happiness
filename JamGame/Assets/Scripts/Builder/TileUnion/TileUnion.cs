using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileUnion : MonoBehaviour
{
    [SerializeField] public GameObject UIPrefab;
    [SerializeField] public List<Tile> tiles = new();
    [SerializeField] Vector2Int unionPosition;
    [SerializeField] int unionRotation;

    public Vector2Int Position
    {
        get { return unionPosition; }
        set {
            unionPosition = value;
            transform.localPosition = new Vector3(
                BuilderMatrix.Step * unionPosition.y,
                transform.localPosition.y,
                -BuilderMatrix.Step * unionPosition.x
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
    public IEnumerable<Vector2Int> TilesPositionsForUpdating
    {
        get 
        {
            HashSet<Vector2Int> buffer = new();
            foreach (var tile in tiles)
            {
                foreach (var position in GetNinePositions())
                {
                    var pos = tile.Position + position;
                    buffer.Add(pos);
                }
            }
            return buffer.Select(arg => new Vector2Int(arg.x + Position.x, arg.y + Position.y));
        }
    }
    public IEnumerable<Vector2Int> TilesPositions
    {
        get { return tiles.Select(x => x.Position + Position); }
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
    public int TilesCount { get { return tiles.Count; } }
    public Vector2Int CenterTilePosition { get { return GetCenterTilePosition(); } }
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
    public IEnumerable<Vector2Int> GetImaginePlaces(Vector2Int unionPosition, int unionRotation)
    {
        var list = tiles.Select(x => x.Position).ToList();
        // Rotating
        for (int j = Rotation; j < unionRotation; j++)
        {
            var firstCenter = GetCenterOfMass(list);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] = new(list[i].y, -list[i].x);
            }
            var secondCenter = GetCenterOfMass(list);
            var delta = (firstCenter - secondCenter);
            for (int i = 0; i < list.Count; i++)
            {
                list[i] += new Vector2Int((int)delta.x, (int)delta.y);
            }
        }
        return list.Select(x => x + unionPosition);
    }
    public List<Tile> GetOverlappingWalls(TileBuilder tileBuilder)
    {
        List<Tile> invalidTiles = new();
        foreach (var tile in tiles)
        {
            if (!tile.UpdateWalls(tileBuilder, tile.Position + Position).Valid)
                invalidTiles.Add(tile);
        }
        return invalidTiles;
    }
    public void IsolateUpdate()
    {
        // TODO
    }

    void RotateRight()
    {
        unionRotation++;
        var firstCenter = GetCenterOfMass(tiles.Select(x => x.Position).ToList());
        foreach (var tile in tiles)
        {
            tile.Rotation++;
            tile.Position = new Vector2Int(tile.Position.y, -tile.Position.x);
        } 
        unionRotation %= 4;
        var secondCenter = GetCenterOfMass(tiles.Select(x => x.Position).ToList());
        var delta = (firstCenter - secondCenter);
        foreach (var tile in tiles)
        {
            tile.Position += new Vector2Int((int)delta.x, (int)delta.y);
        }
    }
    Vector2 GetCenterOfMass(List<Vector2Int> positions)
    {
        Vector2 VectorSum = new();
        foreach (var pos in positions)
        {
            VectorSum += pos;
        }
        VectorSum /= positions.Count;
        return GetNearestPointForCenterOfMass(VectorSum);
    }
    Vector2 GetNearestPointForCenterOfMass(Vector2 vector)
    {
        List<Vector2> variants = new();
        variants.Add(new(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y)));
        variants.Add(
            new(
                Mathf.RoundToInt(vector.x + vector.normalized.x / 2) - vector.normalized.x / 2,
                Mathf.RoundToInt(vector.y + vector.normalized.y / 2) - vector.normalized.x / 2
            ));
        return variants.OrderBy(x => Vector2.Distance(x, vector)).First();
    }
    Vector2Int GetCenterTilePosition()
    {
        Vector2 VectorSum = new();
        foreach (var pos in TilesPositions)
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
    IEnumerable<Vector2Int> GetNinePositions()
    {
        for (int i = 1; i >= -1; i--)
        {
            for (int j = -1; j <= 1; j++)
            {
                yield return new Vector2Int(j, i);
            }
        }
    }
}