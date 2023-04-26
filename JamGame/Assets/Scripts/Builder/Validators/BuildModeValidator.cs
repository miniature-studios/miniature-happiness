using Common;
using System.Linq;

public class BuildModeValidator : IValidator
{
    TileBuilder tileBuilder;
    public BuildModeValidator(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }
    public bool ValidateCommand(ICommand command)
    {
        if(command is AddTileToSceneCommand)
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
                    if (insideListPositions.Intersect(futurePlaces).Count() == creatingtileUnion.TileCount 
                        && tileBuilder.IsValidPlacing(creatingtileUnion, freePosition, rotation))
                    {
                        addCommand.CreatingPosition = freePosition;
                        addCommand.CreatingRotation = rotation;
                        return true;
                    }
                }
                rotation++;
            }
            return false;
        }
        if (command is SelectTileCommand)
        {
            var selectCommand = command as SelectTileCommand;
            TileUnion selecredTileUnion = tileBuilder.DetectTileUnion(selectCommand.tile);
            if (selecredTileUnion.IsAllWithMark("immutable") || selecredTileUnion.IsAllWithMark("freecpace"))
                return false;
            return true;
        }
        if (command is MoveSelectedTileCommand)
        {
            if (tileBuilder.SelectedTile == null)
                return false;
            var moveCommand = command as MoveSelectedTileCommand;
            var newUnionPosition = tileBuilder.SelectedTile.Position + moveCommand.direction.ToVector2Int();
            var newPositions = tileBuilder.SelectedTile.GetImaginePlaces(newUnionPosition, tileBuilder.SelectedTile.Rotation);
            if (tileBuilder.GetTileUnionsInPositions(newPositions).All(x => !x.IsAllWithMark("outside"))
                && tileBuilder.IsValidPlacing(tileBuilder.SelectedTile, newUnionPosition, tileBuilder.SelectedTile.Rotation))
                return true;
            return false;
        }
        if (command is ComplatePlacingCommand)
        {
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
            var rotateCommand = command as RotateSelectedTileCommand;
            var newPosition = tileBuilder.SelectedTile.GetImaginePlaces(tileBuilder.SelectedTile.Position, tileBuilder.SelectedTile.Rotation + 1);
            if (tileBuilder.GetTileUnionsInPositions(newPosition).All(x => !x.IsAllWithMark("outside"))
                && tileBuilder.IsValidPlacing(tileBuilder.SelectedTile, tileBuilder.SelectedTile.Position, tileBuilder.SelectedTile.Rotation+1))
                return true;
            return false;
        }
        return false;
    }
}
