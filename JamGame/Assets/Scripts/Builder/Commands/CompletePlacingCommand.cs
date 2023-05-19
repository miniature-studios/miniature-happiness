using Common;
public class CompletePlacingCommand : ICommand
{
    public Result Execute(TileBuilderController tile_builder_controller)
    {
        return tile_builder_controller.TileBuilder.ComletePlacing();
    }
}