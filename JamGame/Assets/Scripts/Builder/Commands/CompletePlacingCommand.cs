using Common;
public class CompletePlacingCommand : ICommand
{
    public Result Execute(TileBuilderController tileBuilderController)
    {
        return tileBuilderController.TileBuilder.ComletePlacing();
    }
}