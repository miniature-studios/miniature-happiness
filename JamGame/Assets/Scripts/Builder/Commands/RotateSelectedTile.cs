using Common;

public class RotateSelectedTileCommand : ICommand
{
    public RotationDirection Direction { get; }

    public RotateSelectedTileCommand(RotationDirection direction)
    {
        Direction = direction;
    }

    public Result Execute(TileBuilder tile_builder)
    {
        return tile_builder.RotateSelectedTile(Direction);
    }
}
