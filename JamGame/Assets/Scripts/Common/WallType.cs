using UnityEngine;

namespace Common
{
    public enum WallType
    {
        Wall = 1,
        Window = 2,
        Door = 3,
        None = 4
    }

    public static class WallTypeTools
    {
        public static bool IsPassable(this WallType type)
        {
            switch (type)
            {
                case WallType.Window:
                case WallType.Wall: return false;
                case WallType.None:
                case WallType.Door: return true;
                default:
                    Debug.LogError("Invald wall type");
                    return false;
            }
        }
    }
}