using Common;
using System.Linq;
using UnityEngine;

public class GodModeValidator : IValidator
{
    private readonly TileBuilder tileBuilder;
    public GodModeValidator(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }

    public Result ValidateCommand(ICommand command)
    {
        if (command is AddTileToSceneCommand)
        {
            AddTileToSceneCommand addCommand = command as AddTileToSceneCommand;
            TileUnion creatingtileUnion = addCommand.tilePrefab.GetComponent<TileUnion>();
            System.Collections.Generic.IEnumerable<Vector2Int> insideListPositions = tileBuilder.GetInsideListPositions();
            int rotation = 0;
            while (rotation < 4)
            {
                foreach (Vector2Int freePosition in insideListPositions)
                {
                    System.Collections.Generic.IEnumerable<Vector2Int> futurePlaces = creatingtileUnion.GetImaginePlaces(freePosition, creatingtileUnion.Rotation + rotation);
                    if (insideListPositions.Intersect(futurePlaces).Count() == creatingtileUnion.TilesCount)
                    {
                        addCommand.CreatingPosition = freePosition;
                        addCommand.CreatingRotation = rotation;
                        return new SuccessResult();
                    }
                }
                rotation++;
            }
            Vector2Int position = new(0, 0);
            do
            {
                position.x++;
            } while (tileBuilder.GetTileUnionsInPositions(creatingtileUnion.GetImaginePlaces(position, creatingtileUnion.Rotation)).Count() > 0);
            addCommand.CreatingPosition = position;
            addCommand.CreatingRotation = creatingtileUnion.Rotation;
            return new SuccessResult();
        }
        return new SuccessResult();
    }
}

