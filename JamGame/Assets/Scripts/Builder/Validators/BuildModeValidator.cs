using Common;
using System.Linq;
using UnityEngine;

public class BuildModeValidator : IValidator
{
    TileBuilder tileBuilder;
    public BuildModeValidator(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }
    public Result ValidateCommand(ICommand command)
    {
        if (command is AddTileToSceneCommand)
        {
            var addCommand = command as AddTileToSceneCommand;
            TileUnion creatingtileUnion = addCommand.tilePrefab.GetComponent<TileUnion>();
            var insideListPositions = tileBuilder.GetInsideListPositions();
            int rotation = 0;
            bool choosed = false;
            Vector2Int pickedPosition;
            var result = tileBuilder.BuilderMatrix.GetMatrixPosition(addCommand.ray);
            if (result.Success)
            {
                pickedPosition = (result as Result<Vector2Int>).Data;
            }
            else
            {
                pickedPosition = Vector2Int.zero;
            }
            Vector2Int bufferPosition = Vector2Int.zero;
            int bufferRotation = 0;
            float bufferDictance = float.MaxValue;
            while (rotation < 4)
            {
                foreach (var freePosition in insideListPositions)
                {
                    var futurePlaces = creatingtileUnion.GetImaginePlaces(freePosition, creatingtileUnion.Rotation + rotation);
                    if (insideListPositions.Intersect(futurePlaces).Count() == creatingtileUnion.TilesCount)
                    {
                        choosed = true;
                        var calcDictance = Vector2.Distance(TileUnionTools.GetCenterOfMass(futurePlaces.ToList()), pickedPosition);
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
            var selectCommand = command as SelectTileCommand;
            if (selectCommand.tile == null)
            {
                return new FailResult("No hits");
            }
            if (selectCommand.tile.IsAllWithMark("immutable"))
            {
                return new FailResult("Immutable Tile");
            }
            if (selectCommand.tile.IsAllWithMark("freespace"))
            {
                return new FailResult("Free space Tile");
            }
            return new SuccessResult();
        }
        if (command is MoveSelectedTileToRayCommand)
        { 
            if (tileBuilder.SelectedTile == null)
            {
                return new FailResult("Not selected Tile");
            }
            return new SuccessResult();
        }
        if (command is MoveSelectedTileCommand)
        {
            if (tileBuilder.SelectedTile == null)
            {
                return new FailResult("Not selected Tile");
            }
            var moveCommand = command as MoveSelectedTileCommand;
            var newUnionPosition = tileBuilder.SelectedTile.Position + moveCommand.direction.ToVector2Int();
            var newPositions = tileBuilder.SelectedTile.GetImaginePlaces(newUnionPosition, tileBuilder.SelectedTile.Rotation);
            if (!tileBuilder.GetTileUnionsInPositions(newPositions).All(x => !x.IsAllWithMark("outside")))
            {
                return new FailResult("Can not move outside");
            }
            return new SuccessResult();
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
            var rotateCommand = command as RotateSelectedTileCommand;
            var newPosition = tileBuilder.SelectedTile.GetImaginePlaces(tileBuilder.SelectedTile.Position, tileBuilder.SelectedTile.Rotation + rotateCommand.direction.GetIntRotationValue());
            if (!tileBuilder.GetTileUnionsInPositions(newPosition).All(x => !x.IsAllWithMark("outside")))
            {
                return new FailResult("Can not rotate into outside");
            }
            return new SuccessResult();
        }
        return new FailResult("Can not do this command");
    }
}
