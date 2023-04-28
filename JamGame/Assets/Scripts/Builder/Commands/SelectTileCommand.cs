using Common;

public class SelectTileCommand : ICommand
{
    TileBuilder tileBuilder;
    public Tile tile;
    public SelectTileCommand(TileBuilder tileBuilder, Tile tile)
    {
        this.tileBuilder = tileBuilder;
        this.tile = tile;
    }
    public Response Execute()
    {
        return tileBuilder.SelectTile(tile);
    }
}