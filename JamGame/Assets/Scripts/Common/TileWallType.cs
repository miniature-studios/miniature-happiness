namespace Common
{
    public enum TileWallType
    {
        Window,
        Wall,
        Door,
        None
    }

    public static class TileWallTypeTools
    {
        public static bool IsPassable(this TileWallType wallType)
        {
            return wallType is TileWallType.None or TileWallType.Door;
        }

        public static bool IsWall(this TileWallType wallType)
        {
            return wallType is not TileWallType.None;
        }
    }
}
