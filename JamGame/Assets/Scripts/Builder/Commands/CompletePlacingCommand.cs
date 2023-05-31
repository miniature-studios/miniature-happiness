using Common;
public class CompletePlacingCommand : ICommand
{
    public Result Execute(TileBuilder tile_builder)
    {
        return tile_builder.ComletePlacing();
    }
}