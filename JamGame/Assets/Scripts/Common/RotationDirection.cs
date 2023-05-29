namespace Common
{
    public enum RotationDirection
    {
        Right = 1,
        Left = -1
    }
    public static class RotationDirectionTools
    {
        public static int ToInt(this RotationDirection direction)
        {
            return (int)direction;
        }
    }
}
