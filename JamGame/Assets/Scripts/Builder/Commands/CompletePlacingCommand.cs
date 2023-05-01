using Common;
public class CompletePlacingCommand : ICommand
{
    TileBuilder tileBuilder;
    public CompletePlacingCommand(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }
    public Response Execute()
    {
        return tileBuilder.ComletePlacing();
    }
}