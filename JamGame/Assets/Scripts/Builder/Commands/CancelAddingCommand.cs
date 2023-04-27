
public class CancelAddingCommand : ICommand
{
    TileBuilder tileBuilder;
    public CancelAddingCommand(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }
    public Answer Execute()
    {
        return tileBuilder.ComletePlacing();
    }
}