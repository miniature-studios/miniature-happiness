using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class TileUnionTools
{
    public static Vector2 GetCenterOfMass(List<Vector2Int> positions)
    {
        Vector2 VectorSum = new();
        foreach (var pos in positions)
        {
            VectorSum += pos;
        }
        VectorSum /= positions.Count;
        List<Vector2> variants = new();
        variants.Add(new(Mathf.RoundToInt(VectorSum.x), Mathf.RoundToInt(VectorSum.y)));
        variants.Add(new(
        Mathf.RoundToInt(VectorSum.x + VectorSum.normalized.x / 2) - VectorSum.normalized.x / 2,
        Mathf.RoundToInt(VectorSum.y + VectorSum.normalized.y / 2) - VectorSum.normalized.x / 2
        ));
        return variants.OrderBy(x => Vector2.Distance(x, VectorSum)).First();
    }
}