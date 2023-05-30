using System.Collections.Generic;
using UnityEngine;

namespace Common
{
    public enum Direction
    {
        Up = 0,
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
            return (Direction)(((int)dir + 2) % 8);
        }
        public static Direction RotateMinus90(this Direction dir)
        {
            int buffer = (int)dir - 2;
            return (Direction)(buffer < 0 ? 8 + buffer : buffer);
        }
        public static Direction Rotate45(this Direction dir)
        {
            return (Direction)(((int)dir + 1) % 8);
        }
        public static Direction RotateMinus45(this Direction dir)
        {
            int buffer = (int)dir - 1;
            return (Direction)(buffer < 0 ? 8 - buffer : buffer);
        }
        public static Direction GetOpposite(this Direction dir)
        {
            return (Direction)(((int)dir + 4) % 8);
        }
        public static float GetDegrees(this Direction dir)
        {
            return (int)dir * 45;
        }
        public static IEnumerable<Direction> GetCircle90(this Direction dir)
        {
            List<Direction> list = new();
            for (int i = 0; i < 4; i++)
            {
                list.Add(dir);
                dir = dir.Rotate90();
            }
            return list;
        }
        public static IEnumerable<Direction> GetCircle45(this Direction dir)
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