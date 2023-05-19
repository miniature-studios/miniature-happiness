using Common;
public class RotateSelectedTileCommand : ICommand
{
    public Direction Direction;
    public RotateSelectedTileCommand(Direction direction)
    {
        Direction = direction;
    }
    public Result Execute(TileBuilderController tile_builder_controller)
    {
        return tile_builder_controller.TileBuilder.RotateSelectedTile(Direction);
    }
}