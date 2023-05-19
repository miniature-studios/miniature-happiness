using Common;
using System.Collections.Generic;
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
            AddTileToSceneCommand add_command = command as AddTileToSceneCommand;
            TileUnion creatingtile_union = add_command.TilePrefab.GetComponent<TileUnion>();
            IEnumerable<Vector2Int> inside_list_positions = tileBuilder.GetInsideListPositions();
            int rotation = 0;
            while (rotation < 4)
            {
                foreach (Vector2Int free_position in inside_list_positions)
                {
                    IEnumerable<Vector2Int> futurePlaces = creatingtile_union.GetImaginePlaces(free_position, creatingtile_union.Rotation + rotation);
                    if (inside_list_positions.Intersect(futurePlaces).Count() == creatingtile_union.TilesCount)
                    {
                        add_command.CreatingPosition = free_position;
                        add_command.CreatingRotation = rotation;
                        return new SuccessResult();
                    }
                }
                rotation++;
            }
            Vector2Int position = new(0, 0);
            do
            {
                position.x++;
            } while (tileBuilder.GetTileUnionsInPositions(creatingtile_union.GetImaginePlaces(position, creatingtile_union.Rotation)).Count() > 0);
            add_command.CreatingPosition = position;
            add_command.CreatingRotation = creatingtile_union.Rotation;
            return new SuccessResult();
        }
        return new SuccessResult();
    }
}

