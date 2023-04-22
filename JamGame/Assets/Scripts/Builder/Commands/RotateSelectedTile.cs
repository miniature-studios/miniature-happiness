
public class RotateSelectedTileCommand : ICommand
{
    TileBuilder tileBuilder;
    public RotateSelectedTileCommand(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }
    public void Execute()
    {
        tileBuilder.RotateSelectedTile();
    }
}