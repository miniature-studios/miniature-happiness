using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class BuilderMatrix
{
    public static float SelectingPlaneHeight = 1;
    public static int Step = 5;
    public static Vector2Int GetMatrixPosition(Vector2 point)
    {
        point /= Step;
        point = new(-point.y, point.x);
        List<Vector2Int> positions = new();
        positions.Add(new(RoundDown(point.x), RoundDown(point.y)));
        positions.Add(new(RoundDown(point.x), RoundUp(point.y)));
        positions.Add(new(RoundUp(point.x), RoundDown(point.y)));
        positions.Add(new(RoundUp(point.x), RoundUp(point.y)));
        return positions.OrderBy(x => Vector2.Distance(point, (Vector2)x)).First();
    }
    static int RoundDown(float value)
    {
        return (int)Math.Truncate(value);
    }
    static int RoundUp(float value)
    {
        return (int)Math.Truncate(value) + 1;
    }
}

