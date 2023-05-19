using Common;

public class MoveSelectedTileCommand : ICommand
{
    public Direction Direction;
    public MoveSelectedTileCommand(Direction direction)
    {
        Direction = direction;
    }
    public Result Execute(TileBuilderController tile_builder_controller)
    {
        return tile_builder_controller.TileBuilder.MoveSelectedTile(Direction);
    }
}