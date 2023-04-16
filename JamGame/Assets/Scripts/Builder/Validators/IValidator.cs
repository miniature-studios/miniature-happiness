using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IValidator
{
    public bool ValidateCommand(ICommand command);
}

