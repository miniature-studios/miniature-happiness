﻿using UnityEngine;
using System.Linq;

public class GodModeValidator : IValidator
{
    TileBuilder tileBuilder;
    public GodModeValidator(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }

    public Answer ValidateCommand(ICommand command)
    {
        if (command is AddTileToSceneCommand)
        {
            var addCommand = command as AddTileToSceneCommand;
            TileUnion creatingtileUnion = addCommand.TilePrefab.GetComponent<TileUnion>();
            var insideListPositions = tileBuilder.GetInsideListPositions();
            int rotation = 0;
            while (rotation < 4)
            {
                foreach (var freePosition in insideListPositions)
                {
                    var futurePlaces = creatingtileUnion.GetImaginePlaces(freePosition, creatingtileUnion.Rotation + rotation);
                    if (insideListPositions.Intersect(futurePlaces).Count() == creatingtileUnion.TileCount)
                    {
                        addCommand.CreatingPosition = freePosition;
                        addCommand.CreatingRotation = rotation;
                        return new Answer("Accepted", true);
                    }
                }
                rotation++;
            }
            Vector2Int position = new(0, 0);
            do
            {
                position.x++;
            } while (tileBuilder.GetTileUnionsInPositions(creatingtileUnion.GetImaginePlaces(position, creatingtileUnion.Rotation)).Count > 0);
            addCommand.CreatingPosition = position;
            addCommand.CreatingRotation = creatingtileUnion.Rotation;
            return new Answer("Accepted", true);
        }
        return new Answer("Accepted", true);
    }
}

