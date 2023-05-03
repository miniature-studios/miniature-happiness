using Common;
public class CompletePlacingCommand : ICommand
{
    TileBuilder tileBuilder;
    public CompletePlacingCommand(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }
    public Result Execute()
    {
        return tileBuilder.ComletePlacing();
    }
}