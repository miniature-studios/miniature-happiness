using System;
using UnityEngine;
using Common;

public class GameModeValidator : IValidator
{
    public Answer ValidateCommand(ICommand command)
    {
        return new Answer("Cannot do anything in Game Mode", false);
    }
}
