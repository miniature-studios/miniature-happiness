﻿using Common;
using System.Collections.Generic;
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
        if (command is CompletePlacingCommand or DeleteSelectedTileCommand or ValidateBuildingCommand)
        {
            return new SuccessResult();
        }
        if (command is AddTileToSceneCommand add_command)
        {
            TileUnion creatingtile_union = add_command.TilePrefab.GetComponent<TileUnion>();
            IEnumerable<Vector2Int> inside_list_positions = tileBuilder.GetFreeSpaceInsideListPositions();

            Vector2Int picked_position;
            Result<Vector2Int> result = tileBuilder.BuilderMatrix.GetMatrixPosition(add_command.Ray);
            picked_position = result.Success ? result.Data : Vector2Int.zero;

            int rotation = 0;
            bool choosed = false;
            Vector2Int buffer_position = Vector2Int.zero;
            int buffer_rotation = 0;
            float buffer_dictance = float.MaxValue;

            while (rotation < 4)
            {
                foreach (Vector2Int free_position in inside_list_positions)
                {
                    IEnumerable<Vector2Int> future_places = creatingtile_union.GetImaginePlaces(free_position, creatingtile_union.Rotation + rotation);
                    if (inside_list_positions.Intersect(future_places).Count() == creatingtile_union.TilesCount)
                    {
                        choosed = true;
                        float calc_dictance = Vector2.Distance(TileUnionTools.GetCenterOfMass(future_places.ToList()), picked_position);
                        if (calc_dictance < buffer_dictance)
                        {
                            buffer_position = free_position;
                            buffer_rotation = rotation;
                            buffer_dictance = calc_dictance;
                        }
                    }
                }
                rotation++;
            }
            if (choosed)
            {
                add_command.CreatingPosition = buffer_position;
                add_command.CreatingRotation = buffer_rotation;
                return new SuccessResult();
            }
            else
            {
                return new FailResult("Cannot place, not enough free place");
            }
        }
        if (command is SelectTileCommand select_command)
        {
            return select_command.Tile == null
                ? new FailResult("No hits")
                : select_command.Tile.IsAllWithMark("Immutable")
                ? new FailResult("Immutable Tile")
                : select_command.Tile.IsAllWithMark("freespace") ? new FailResult("Free space Tile") : new SuccessResult();
        }
        if (command is MoveSelectedTileCommand move_command)
        {
            if (tileBuilder.SelectedTile == null)
            {
                return new FailResult("Not selected Tile");
            }
            if (move_command.Direction == null)
            {
                return new FailResult("Null direction");
            }

            Vector2Int new_union_position = tileBuilder.SelectedTile.Position + move_command.Direction.Value.ToVector2Int();
            IEnumerable<Vector2Int> newPositions = tileBuilder.SelectedTile.GetImaginePlaces(new_union_position, tileBuilder.SelectedTile.Rotation);
            return !tileBuilder.GetTileUnionsInPositions(newPositions).All(x => !x.IsAllWithMark("Outside"))
                ? new FailResult("Can not move outside")
                : new SuccessResult();
        }
        if (command is RotateSelectedTileCommand rotate_command)
        {
            if (tileBuilder.SelectedTile == null)
            {
                return new FailResult("Not selected Tile");
            }
            IEnumerable<Vector2Int> newPosition = tileBuilder.SelectedTile.GetImaginePlaces(tileBuilder.SelectedTile.Position, tileBuilder.SelectedTile.Rotation + rotate_command.Direction.ToInt());
            return !tileBuilder.GetTileUnionsInPositions(newPosition).All(x => !x.IsAllWithMark("Outside"))
                ? new FailResult("Can not rotate into outside")
                : new SuccessResult();
        }
        return new FailResult("Can not do this command");
    }
}
