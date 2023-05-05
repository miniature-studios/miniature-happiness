using Common;

public class GameModeValidator : IValidator
{
    public Result ValidateCommand(ICommand command)
    {
        return new FailResult("Cannot do anything in Game Mode");
    }
}
