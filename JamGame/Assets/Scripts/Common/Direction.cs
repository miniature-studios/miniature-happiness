using UnityEngine;

namespace Common
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        Center
    }

    public static class DirectionTools
    {
        public static Direction ToDirection(this Vector2Int vec)
        {
            switch ((vec.x, vec.y))
            {
                case (0, 1): return Direction.Up;
                case (0, -1): return Direction.Down;
                case (-1, 0): return Direction.Left;
                case (1, 0): return Direction.Right;
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
                case Direction.Down: return new Vector2Int(0, -1);
                case Direction.Left: return new Vector2Int(-1, 0);
                case Direction.Right: return new Vector2Int(1, 0);
                case Direction.Center: return new Vector2Int(0, 0);
                default:
                    Debug.LogError("Unknown Direction");
                    return Vector2Int.zero;
            }
        }
    }
}
