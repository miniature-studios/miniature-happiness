using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common
{
    public static class CenterOfMassTools
    {
        public static Vector2 GetCenterOfMass(this IEnumerable<Vector2Int> positions)
        {
            Vector2 vectorSum = new();
            foreach (Vector2Int pos in positions)
            {
                vectorSum += pos;
            }
            vectorSum /= positions.Count();
            List<Vector2> variants =
                new()
                {
                    new(Mathf.RoundToInt(vectorSum.x), Mathf.RoundToInt(vectorSum.y)),
                    new(
                        Mathf.RoundToInt(vectorSum.x + (Mathf.Sign(vectorSum.normalized.x) / 2))
                            - (Mathf.Sign(vectorSum.normalized.x) / 2),
                        Mathf.RoundToInt(vectorSum.y + (Mathf.Sign(vectorSum.normalized.y) / 2))
                            - (Mathf.Sign(vectorSum.normalized.y) / 2)
                    )
                };
            return variants.OrderBy(x => Vector2.Distance(x, vectorSum)).First();
        }
    }
}
