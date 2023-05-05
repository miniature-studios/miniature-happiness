using Common;
public class CompletePlacingCommand : ICommand
{
    public Result Execute(TileBuilder tileBuilder)
    {
        return tileBuilder.ComletePlacing();
    }
}