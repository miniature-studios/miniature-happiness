using Common;

public class ValidateBuildingCommand : ICommand
{
    public Result Execute(TileBuilder tile_builder)
    {
        return tile_builder.Validate();
    }
}
