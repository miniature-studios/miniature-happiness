using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public enum Direction
    {
        Up,
        RightUp,
        Right,
        RightDown,
        Down,
        LeftDown,
        Left,
        LeftUp,
        Center
    }

    public static class DirectionTools
    {
        public static Direction ToDirection(this Vector2Int vec)
        {
            switch ((vec.x, vec.y))
            {
                case (0, 1): return Direction.Up;
                case (1, 1): return Direction.RightUp;
                case (1, 0): return Direction.Right;
                case (1, -1): return Direction.RightDown;
                case (0, -1): return Direction.Down;
                case (-1, -1): return Direction.LeftDown;
                case (-1, 0): return Direction.Left;
                case (-1, 1): return Direction.LeftUp;
                case (0, 0): return Direction.Center;
                default:
                    Debug.LogError("Invald vector");
                    return Direction.Center;
            }
        }
        public static Vector2Int ToVector2Int(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return new Vector2Int(0, 1);
                case Direction.RightUp: return new Vector2Int(1, 1);
                case Direction.Right: return new Vector2Int(1, 0);
                case Direction.RightDown: return new Vector2Int(1, -1);
                case Direction.Down: return new Vector2Int(0, -1);
                case Direction.LeftDown: return new Vector2Int(-1, -1);
                case Direction.Left: return new Vector2Int(-1, 0);
                case Direction.LeftUp: return new Vector2Int(-1, 1);
                case Direction.Center: return new Vector2Int(0, 0);
                default:
                    Debug.LogError("Unknown Direction");
                    return Vector2Int.zero;
            }
        }
        public static Direction Rotate90(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return Direction.Right;
                case Direction.RightUp: return Direction.RightDown;
                case Direction.Right: return Direction.Down;
                case Direction.RightDown: return Direction.LeftDown;
                case Direction.Down: return Direction.Left;
                case Direction.LeftDown: return Direction.LeftUp;
                case Direction.Left: return Direction.Up;
                case Direction.LeftUp: return Direction.RightUp;
                case Direction.Center: return Direction.Center;
                default:
                    Debug.LogError("Unknown Direction");
                    return dir;
            }
        }
        public static Direction RotateMinus90(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return Direction.Left;
                case Direction.RightUp: return Direction.LeftUp;
                case Direction.Right: return Direction.Up;
                case Direction.RightDown: return Direction.RightUp;
                case Direction.Down: return Direction.Right;
                case Direction.LeftDown: return Direction.RightDown;
                case Direction.Left: return Direction.Down;
                case Direction.LeftUp: return Direction.LeftDown;
                case Direction.Center: return Direction.Center;
                default:
                    Debug.LogError("Unknown Direction");
                    return dir;
            }
        }
        public static Direction Rotate45(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return Direction.RightUp;
                case Direction.RightUp: return Direction.Right;
                case Direction.Right: return Direction.RightDown;
                case Direction.RightDown: return Direction.Down;
                case Direction.Down: return Direction.LeftDown;
                case Direction.LeftDown: return Direction.Left;
                case Direction.Left: return Direction.LeftUp;
                case Direction.LeftUp: return Direction.Up;
                case Direction.Center: return Direction.Center;
                default:
                    Debug.LogError("Unknown Direction");
                    return dir;
            }
        }
        public static Direction RotateMinus45(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return Direction.LeftUp;
                case Direction.RightUp: return Direction.Up;
                case Direction.Right: return Direction.RightUp;
                case Direction.RightDown: return Direction.Right;
                case Direction.Down: return Direction.RightDown;
                case Direction.LeftDown: return Direction.Down;
                case Direction.Left: return Direction.LeftDown;
                case Direction.LeftUp: return Direction.Left;
                case Direction.Center: return Direction.Center;
                default:
                    Debug.LogError("Unknown Direction");
                    return dir;
            }
        }
        public static Direction GetOpposite(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return Direction.Down;
                case Direction.RightUp: return Direction.LeftDown;
                case Direction.Right: return Direction.Left;
                case Direction.RightDown: return Direction.LeftUp;
                case Direction.Down: return Direction.Up;
                case Direction.LeftDown: return Direction.RightUp;
                case Direction.Left: return Direction.Right;
                case Direction.LeftUp: return Direction.RightDown;
                case Direction.Center: return Direction.Center;
                default:
                    Debug.LogError("Unknown Direction");
                    return Direction.Center;
            }
        }
        public static float GetDegrees(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return 0;
                case Direction.RightUp: return 45;
                case Direction.Right: return 90;
                case Direction.RightDown: return 135;
                case Direction.Down: return 180;
                case Direction.LeftDown: return 225;
                case Direction.Left: return 270;
                case Direction.LeftUp: return 315;
                case Direction.Center: return 0;
                default:
                    Debug.LogError("Unknown Direction");
                    return 0;
            }
        }
        public static int GetIntRotationValue(this Direction dir)
        {
            switch (dir)
            {
                case Direction.Up: return 0;
                case Direction.RightUp: return 0;
                case Direction.Right: return 1;
                case Direction.RightDown: return 0;
                case Direction.Down: return 0;
                case Direction.LeftDown: return 0;
                case Direction.Left: return -1;
                case Direction.LeftUp: return 0;
                case Direction.Center: return 0;
                default:
                    Debug.LogError("Unknown Direction");
                    return 0;
            }
        }
        public static List<Direction> GetCircle90(this Direction dir)
        {
            List<Direction> list = new();
            for (int i = 0; i < 4; i++)
            {
                list.Add(dir);
                dir = dir.Rotate90();
            }
            return list;
        }
        public static List<Direction> GetCircle45(this Direction dir)
        {
            List<Direction> list = new();
            for (int i = 0; i < 8; i++)
            {
                list.Add(dir);
                dir = dir.Rotate45();
            }
            return list;
        }
    }
}