
public class ComplatePlacingCommand : ICommand
{
    TileBuilder tileBuilder;
    public ComplatePlacingCommand(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }
    public void Execute()
    {
        tileBuilder.ComletePlacing();
    }
}