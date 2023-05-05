using Common;
using System.Linq;
using UnityEngine;

public class BuildModeValidator : IValidator
{
    private readonly TileBuilder tileBuilder;
    public BuildModeValidator(TileBuilder tileBuilder)
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
            bool choosed = false;
            Vector2Int pickedPosition;
            Result<Vector2Int> result = tileBuilder.BuilderMatrix.GetMatrixPosition(addCommand.ray);
            pickedPosition = result.Success ? result.Data : Vector2Int.zero;
            Vector2Int bufferPosition = Vector2Int.zero;
            int bufferRotation = 0;
            float bufferDictance = float.MaxValue;
            while (rotation < 4)
            {
                foreach (Vector2Int freePosition in insideListPositions)
                {
                    System.Collections.Generic.IEnumerable<Vector2Int> futurePlaces = creatingtileUnion.GetImaginePlaces(freePosition, creatingtileUnion.Rotation + rotation);
                    if (insideListPositions.Intersect(futurePlaces).Count() == creatingtileUnion.TilesCount)
                    {
                        choosed = true;
                        float calcDictance = Vector2.Distance(TileUnionTools.GetCenterOfMass(futurePlaces.ToList()), pickedPosition);
                        if (calcDictance < bufferDictance)
                        {
                            bufferPosition = freePosition;
                            bufferRotation = rotation;
                            bufferDictance = calcDictance;
                        }
                    }
                }
                rotation++;
            }
            if (choosed)
            {
                addCommand.CreatingPosition = bufferPosition;
                addCommand.CreatingRotation = bufferRotation;
                return new SuccessResult();
            }
            else
            {
                return new FailResult("Cannot place, not enough free place");
            }
        }
        if (command is SelectTileCommand)
        {
            SelectTileCommand selectCommand = command as SelectTileCommand;
            return selectCommand.tile == null
                ? new FailResult("No hits")
                : selectCommand.tile.IsAllWithMark("immutable")
                ? new FailResult("Immutable Tile")
                : selectCommand.tile.IsAllWithMark("freespace") ? new FailResult("Free space Tile") : new SuccessResult();
        }
        if (command is MoveSelectedTileToRayCommand)
        {
            return tileBuilder.SelectedTile == null ? new FailResult("Not selected Tile") : new SuccessResult();
        }
        if (command is MoveSelectedTileCommand)
        {
            if (tileBuilder.SelectedTile == null)
            {
                return new FailResult("Not selected Tile");
            }
            MoveSelectedTileCommand moveCommand = command as MoveSelectedTileCommand;
            Vector2Int newUnionPosition = tileBuilder.SelectedTile.Position + moveCommand.direction.ToVector2Int();
            System.Collections.Generic.IEnumerable<Vector2Int> newPositions = tileBuilder.SelectedTile.GetImaginePlaces(newUnionPosition, tileBuilder.SelectedTile.Rotation);
            return !tileBuilder.GetTileUnionsInPositions(newPositions).All(x => !x.IsAllWithMark("outside"))
                ? new FailResult("Can not move outside")
                : new SuccessResult();
        }
        if (command is CompletePlacingCommand)
        {
            return new SuccessResult();
        }
        if (command is DeleteSelectedTileCommand)
        {
            return new SuccessResult();
        }
        if (command is RotateSelectedTileCommand)
        {
            if (tileBuilder.SelectedTile == null)
            {
                return new FailResult("Not selected Tile");
            }
            RotateSelectedTileCommand rotateCommand = command as RotateSelectedTileCommand;
            System.Collections.Generic.IEnumerable<Vector2Int> newPosition = tileBuilder.SelectedTile.GetImaginePlaces(tileBuilder.SelectedTile.Position, tileBuilder.SelectedTile.Rotation + rotateCommand.direction.GetIntRotationValue());
            return !tileBuilder.GetTileUnionsInPositions(newPosition).All(x => !x.IsAllWithMark("outside"))
                ? new FailResult("Can not rotate into outside")
                : new SuccessResult();
        }
        return new FailResult("Can not do this command");
    }
}
