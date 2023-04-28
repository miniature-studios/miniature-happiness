using Common;
using System.Linq;

public class BuildModeValidator : IValidator
{
    TileBuilder tileBuilder;
    public BuildModeValidator(TileBuilder tileBuilder)
    {
        this.tileBuilder = tileBuilder;
    }
    public Response ValidateCommand(ICommand command)
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
                    if (insideListPositions.Intersect(futurePlaces).Count() == creatingtileUnion.TilesCount)
                    {
                        addCommand.CreatingPosition = freePosition;
                        addCommand.CreatingRotation = rotation;
                        return new Response("Accepted", true);
                    }
                }
                rotation++;
            }
            return new Response("Cannot place, not enough free place", false);
        }
        if (command is SelectTileCommand)
        {
            var selectCommand = command as SelectTileCommand;
            TileUnion selectedTileUnion = tileBuilder.DetectTileUnion(selectCommand.tile);
            if (selectedTileUnion.IsAllWithMark("immutable"))
                return new Response("Immutable Tile", false);
            if (selectedTileUnion.IsAllWithMark("freecpace"))
                return new Response("Free cpace Tile", false);
            return new Response("Accepted", true);
        }
        if(command is MoveSelectedTileToRayCommand)
        {
            if (tileBuilder.SelectedTile == null)
            {
                return new Response("Not selected Tile", false);
            }
            return new Response("Accepted", true);
        }
        if (command is MoveSelectedTileCommand)
        {
            if (tileBuilder.SelectedTile == null)
            {
                return new Response("Not selected Tile", false);
            }
            var moveCommand = command as MoveSelectedTileCommand;
            var newUnionPosition = tileBuilder.SelectedTile.Position + moveCommand.direction.ToVector2Int();
            var newPositions = tileBuilder.SelectedTile.GetImaginePlaces(newUnionPosition, tileBuilder.SelectedTile.Rotation);
            if (!tileBuilder.GetTileUnionsInPositions(newPositions).All(x => !x.IsAllWithMark("outside")))
                return new Response("Can not move outside", false);
            return new Response("Accepted", true);
        }
        if (command is ComplatePlacingCommand)
        {
            return new Response("Accepted", true);
        }
        if (command is DeleteSelectedTileCommand)
        {
            return new Response("Accepted", true);
        }
        if (command is RotateSelectedTileCommand)
        {
            if (tileBuilder.SelectedTile == null)
            {
                return new Response("Not selected Tile", false);
            }
            var rotateCommand = command as RotateSelectedTileCommand;
            var newPosition = tileBuilder.SelectedTile.GetImaginePlaces(tileBuilder.SelectedTile.Position, tileBuilder.SelectedTile.Rotation + 1);
            if (!tileBuilder.GetTileUnionsInPositions(newPosition).All(x => !x.IsAllWithMark("outside")))
                return new Response("Can not rotate into outside", false);
            return new Response("Accepted", true);
        }
        return new Response("Can not do this command", false);
    }
}
