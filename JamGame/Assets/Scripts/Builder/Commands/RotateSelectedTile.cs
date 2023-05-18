using Common;
public class RotateSelectedTileCommand : ICommand
{
    public Direction direction;
    public RotateSelectedTileCommand(Direction direction)
    {
        this.direction = direction;
    }
    public Result Execute(TileBuilderController tileBuilderController)
    {
        return tileBuilderController.TileBuilder.RotateSelectedTile(direction);
    }
}