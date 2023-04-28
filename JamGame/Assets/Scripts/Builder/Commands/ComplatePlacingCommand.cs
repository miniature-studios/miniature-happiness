using Common;
public class ComplatePlacingCommand : ICommand
{
    TileBuilder tileBuilder;
    public ComplatePlacingCommand(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }
    public Response Execute()
    {
        return tileBuilder.ComletePlacing();
    }
}