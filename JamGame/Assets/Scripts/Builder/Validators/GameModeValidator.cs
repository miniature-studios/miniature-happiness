using System;
using UnityEngine;
using Common;

public class GameModeValidator : IValidator
{
    public bool ValidateCommand(ICommand command)
    {
        return false;
    }
}
