using Common;
public class RotateSelectedTileCommand : ICommand
{
    TileBuilder tileBuilder;
    public Direction direction;
    public RotateSelectedTileCommand(TileBuilder tileBuilder, Direction direction)
    {
        this.tileBuilder = tileBuilder;
        this.direction = direction;
    }
    public Response Execute()
    {
        return tileBuilder.RotateSelectedTile(direction);
    }
}