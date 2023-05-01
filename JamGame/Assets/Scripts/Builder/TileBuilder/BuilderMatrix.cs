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
        return new(Mathf.RoundToInt(point.x), Mathf.RoundToInt(point.y));
    }
}

