using Common;

public class MoveSelectedTileCommand : ICommand
{
    TileBuilder tileBuilder;
    public Direction direction;
    public MoveSelectedTileCommand(TileBuilder tileBuilder, Direction direction)
    {
        this.tileBuilder = tileBuilder;
        this.direction = direction;
    }
    public void Execute()
    {
        tileBuilder.MoveSelectedTile(direction);
    }
}

