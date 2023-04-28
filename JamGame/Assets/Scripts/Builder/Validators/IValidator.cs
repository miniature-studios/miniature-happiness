using Common;

public interface IValidator
{
    public Response ValidateCommand(ICommand command);
}

