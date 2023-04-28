using Common;

public class GameModeValidator : IValidator
{
    public Response ValidateCommand(ICommand command)
    {
        return new Response("Cannot do anything in Game Mode", false);
    }
}
