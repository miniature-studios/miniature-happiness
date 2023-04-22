using System;
using UnityEngine;
using Common;
using System.Reflection.Emit;

public class GodModeValidator : IValidator
{
    TileBuilder tileBuilder;
    public GodModeValidator(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }

    public bool ValidateCommand(ICommand command)
    {
        if (command is MoveSelectedTileCommand)
        {
            if (tileBuilder.SelectedTile == null)
                return false;
            return true;
        }
        if (command is DeleteSelectedTileCommand)
        {
            if (tileBuilder.SelectedTile == null)
                return false;
            return true;
        }
        if (command is RotateSelectedTileCommand)
        {
            if (tileBuilder.SelectedTile == null)
                return false;
            return true;
        }
        return true;
    }
}

