using Common;
public class RotateSelectedTileCommand : ICommand
{
    public Direction direction;
    public RotateSelectedTileCommand(Direction direction)
    {
        this.direction = direction;
    }
    public Result Execute(TileBuilder tileBuilder)
    {
        return tileBuilder.RotateSelectedTile(direction);
    }
}