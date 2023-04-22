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
            if(tileBuilder.GetInsideListPositions().Count == 0)
                return false;
            return true;
        }
        if (command is SelectTileCommand)
        {
            var selectCommand = command as SelectTileCommand;
            if (selectCommand.tile.Marks.Contains("immutable") || selectCommand.tile.Marks.Contains("freecpace"))
                return false;
            return true;
        }
        if (command is MoveSelectedTileCommand)
        {
            if (tileBuilder.SelectedTile == null)
                return false;
            var moveCommand = command as MoveSelectedTileCommand;
            var newPosition = tileBuilder.SelectedTile.Position + moveCommand.direction.ToVector2Int();
            if (tileBuilder.GetTilesInPosition(newPosition).Count == 0)
                return true;
            if(!tileBuilder.CheckTileForMark(tileBuilder.GetTilesInPosition(newPosition).First(), "immutable"))
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
            return true;
        }
        return false;
    }
}
