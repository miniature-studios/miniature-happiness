using Common;

public interface ICommand
{
    public Result Execute(TileBuilder tile_builder);
}
