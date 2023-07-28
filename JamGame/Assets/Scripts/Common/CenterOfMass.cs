using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common
{
    public static class CenterOfMassTools
    {
        public static float NormalizeToOne(float norm)
        {
            return (norm < 0.0f, norm > 0.0f) switch
            {
                (true, _) => -1.0f,
                (_, true) => 1.0f,
                _ => norm
            };
        }

        public static Vector2 GetCenterOfMass(this List<Vector2Int> positions)
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
                        Mathf.RoundToInt(
                            vector_sum.x + (NormalizeToOne(vector_sum.normalized.x) / 2)
                        ) - (NormalizeToOne(vector_sum.normalized.x) / 2),
                        Mathf.RoundToInt(
                            vector_sum.y + (NormalizeToOne(vector_sum.normalized.y) / 2)
                        ) - (NormalizeToOne(vector_sum.normalized.y) / 2)
                    )
                };
            return variants.OrderBy(x => Vector2.Distance(x, vector_sum)).First();
        }
    }
}
