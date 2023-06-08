using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TileUnionTools
{
    public static Vector2 GetCenterOfMass(List<Vector2Int> positions)
    {
        Vector2 vector_sum = new();
        foreach (Vector2Int pos in positions)
        {
            vector_sum += pos;
        }
        vector_sum /= positions.Count;
        List<Vector2> variants =
            new()
            {
                new(Mathf.RoundToInt(vector_sum.x), Mathf.RoundToInt(vector_sum.y)),
                new(
                    Mathf.RoundToInt(vector_sum.x + (vector_sum.normalized.x / 2))
                        - (vector_sum.normalized.x / 2),
                    Mathf.RoundToInt(vector_sum.y + (vector_sum.normalized.y / 2))
                        - (vector_sum.normalized.x / 2)
                )
            };
        return variants.OrderBy(x => Vector2.Distance(x, vector_sum)).First();
    }
}
