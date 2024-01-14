using UnityEngine;

namespace Common
{
    public static class BoundsTools
    {
        public static Vector3 FitInBounds(this Bounds bounds, Vector3 point)
        {
            return new(
                Mathf.Clamp(point.x, bounds.min.x, bounds.max.x),
                Mathf.Clamp(point.y, bounds.min.y, bounds.max.y),
                Mathf.Clamp(point.z, bounds.min.z, bounds.max.z)
            );
        }
    }
}
