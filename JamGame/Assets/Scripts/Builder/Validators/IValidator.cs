using Common;

public interface IValidator
{
    public Result ValidateCommand(ICommand command);
}

