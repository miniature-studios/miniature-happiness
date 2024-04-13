using UnityEngine;

namespace DA_Assets.Shared.Extensions
{
    public static class ColorBlenderExtensions
    {
        public static Color Difference(this Color c1, Color c2, float intensity)
        {
            float r = Mathf.Abs(c1.r - c2.r) * intensity;
            float g = Mathf.Abs(c1.g - c2.g) * intensity;
            float b = Mathf.Abs(c1.b - c2.b) * intensity;

            Color result = new Color(r, g, b, c1.a);
            return result;
        }

        public static Color Multiply(this Color c1, Color c2, float intensity)
        {
            Color result = c1.Lerp(c1 * c2, intensity);
            return result;
        }

        public static Color Overlay(this Color c1, Color c2, float intensity)
        {
            Color blendedColor = c1.Lerp(c2, intensity);
            return blendedColor;
        }
    }
}