using UnityEngine;
using DA_Assets.Shared.Extensions;

namespace DA_Assets.DAG
{
    public class DAColorBlender
    {
        public static Color Blend(Color c1, Color c2, DAColorBlendMode mode, float intensity)
        {
            switch (mode)
            {
                case DAColorBlendMode.Difference:
                    return c1.Difference(c2, intensity);
                case DAColorBlendMode.Overlay:
                    return c1.Overlay(c2, intensity);
                default:
                    return c1.Multiply(c2, intensity);
            }
        }
    }
}
