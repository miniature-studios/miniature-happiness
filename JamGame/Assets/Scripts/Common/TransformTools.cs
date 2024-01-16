using UnityEngine;

namespace Common
{
    public static class TransformTools
    {
        public static void SetXPosition(this Transform transform, float x)
        {
            transform.position = new(x, transform.position.y, transform.position.z);
        }

        public static void SetYPosition(this Transform transform, float y)
        {
            transform.position = new(transform.position.x, y, transform.position.z);
        }

        public static void SetZPosition(this Transform transform, float z)
        {
            transform.position = new(transform.position.x, transform.position.y, z);
        }
    }
}
