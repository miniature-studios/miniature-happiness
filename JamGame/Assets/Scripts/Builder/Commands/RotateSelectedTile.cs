using Common;
public class RotateSelectedTileCommand : ICommand
{
    TileBuilder tileBuilder;
    public RotateSelectedTileCommand(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }
    public Response Execute()
    {
        return tileBuilder.RotateSelectedTile();
    }
}