using UnityEngine;

namespace Common
{
    public static class MathTools
    {
        public const float EPSILON = 0.001f;

        public static bool IsEqualsZero(this float value)
        {
            return Mathf.Abs(value) - EPSILON < 0;
        }
    }
}
