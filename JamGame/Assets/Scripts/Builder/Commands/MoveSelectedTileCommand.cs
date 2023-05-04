using Common;

public class MoveSelectedTileCommand : ICommand
{
    public Direction direction;
    public MoveSelectedTileCommand(Direction direction)
    {
        this.direction = direction;
    }
    public Result Execute(TileBuilder tileBuilder)
    {
        return tileBuilder.MoveSelectedTile(direction);
    }
}