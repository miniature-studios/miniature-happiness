using Common;
using System.Collections.Generic;
using System.Linq;
using TileBuilder.Command;
using TileUnion;
using UnityEngine;

namespace TileBuilder.Validator
{
    public interface IValidator
    {
        public Result ValidateCommand(ICommand command, ref TileUnionImpl selectedTile);
    }

    public class BuildMode : IValidator
    {
        private readonly TileBuilderImpl tileBuilder;

        public BuildMode(TileBuilderImpl tileBuilder)
        {
            this.tileBuilder = tileBuilder;
        }

        public Result ValidateCommand(ICommand command, ref TileUnionImpl selectedTile)
        {
            if (command is CompletePlacing or DeleteSelectedTile or ValidateBuilding)
            {
                return selectedTile == null
                    ? new FailResult("SelectedTile is Null")
                    : new SuccessResult();
            }
            if (command is AddTileToScene add_command)
            {
                if (selectedTile != null)
                {
                    return new FailResult("Complete placing previous tile");
                }
                TileUnionImpl creatingtile_union =
                    add_command.TilePrefab.GetComponent<TileUnionImpl>();
                IEnumerable<Vector2Int> inside_list_positions =
                    tileBuilder.GetFreeSpaceInsideListPositions();

                Vector2Int picked_position;
                Result<Vector2Int> result = tileBuilder.BuilderMatrix.GetMatrixPosition(
                    add_command.Ray
                );
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
                        IEnumerable<Vector2Int> future_places = creatingtile_union.GetImaginePlaces(
                            free_position,
                            creatingtile_union.Rotation + rotation
                        );
                        if (
                            inside_list_positions.Intersect(future_places).Count()
                            == creatingtile_union.TilesCount
                        )
                        {
                            choosed = true;
                            float calc_dictance = Vector2.Distance(
                                CenterOfMassTools.GetCenterOfMass(future_places.ToList()),
                                picked_position
                            );
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
            if (command is SelectTile select_command)
            {
                return (
                    select_command.Tile == selectedTile,
                    select_command.Tile == null,
                    select_command.Tile.IsAllWithMark("Immutable"),
                    select_command.Tile.IsAllWithMark("Freespace")
                ) switch
                {
                    (true, _, _, _) => new FailResult("Selected already selected tile"),
                    (_, true, _, _) => new FailResult("No hits"),
                    (_, _, true, _) => new FailResult("Immutable Tile"),
                    (_, _, _, true) => new FailResult("Free space Tile"),
                    _ => new SuccessResult()
                };
            }
            if (command is MoveSelectedTile move_command)
            {
                if (selectedTile == null)
                {
                    return new FailResult("Not selected Tile");
                }
                if (move_command.Direction == null)
                {
                    return new FailResult("Null direction");
                }

                Vector2Int new_union_position =
                    selectedTile.Position + move_command.Direction.Value.ToVector2Int();
                IEnumerable<Vector2Int> newPositions = selectedTile.GetImaginePlaces(
                    new_union_position,
                    selectedTile.Rotation
                );
                return !tileBuilder
                    .GetTileUnionsInPositions(newPositions)
                    .All(x => !x.IsAllWithMark("Outside"))
                    ? new FailResult("Can not move outside")
                    : new SuccessResult();
            }
            if (command is RotateSelectedTile rotate_command)
            {
                if (selectedTile == null)
                {
                    return new FailResult("Not selected Tile");
                }
                IEnumerable<Vector2Int> newPosition = selectedTile.GetImaginePlaces(
                    selectedTile.Position,
                    selectedTile.Rotation + (int)rotate_command.Direction
                );
                return !tileBuilder
                    .GetTileUnionsInPositions(newPosition)
                    .All(x => !x.IsAllWithMark("Outside"))
                    ? new FailResult("Can not rotate into outside")
                    : new SuccessResult();
            }
            return new FailResult("Can not do this command");
        }
    }

    public class GameMode : IValidator
    {
        public Result ValidateCommand(ICommand command, ref TileUnionImpl selectedTile)
        {
            return new FailResult("Cannot do anything in Game Mode");
        }
    }

    public class GodMode : IValidator
    {
        private readonly TileBuilderImpl tileBuilder;

        public GodMode(TileBuilderImpl tileBuilder)
        {
            this.tileBuilder = tileBuilder;
        }

        public Result ValidateCommand(ICommand command, ref TileUnionImpl selectedTile)
        {
            if (command is AddTileToScene add_command)
            {
                if (selectedTile != null)
                {
                    return new FailResult("Complete placing previous tile");
                }
                TileUnionImpl creatingtile_union =
                    add_command.TilePrefab.GetComponent<TileUnionImpl>();
                IEnumerable<Vector2Int> inside_list_positions =
                    tileBuilder.GetFreeSpaceInsideListPositions();
                int rotation = 0;
                while (rotation < 4)
                {
                    foreach (Vector2Int free_position in inside_list_positions)
                    {
                        IEnumerable<Vector2Int> futurePlaces = creatingtile_union.GetImaginePlaces(
                            free_position,
                            creatingtile_union.Rotation + rotation
                        );
                        if (
                            inside_list_positions.Intersect(futurePlaces).Count()
                            == creatingtile_union.TilesCount
                        )
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
                } while (
                    tileBuilder
                        .GetTileUnionsInPositions(
                            creatingtile_union.GetImaginePlaces(
                                position,
                                creatingtile_union.Rotation
                            )
                        )
                        .Count() > 0
                );
                add_command.CreatingPosition = position;
                add_command.CreatingRotation = creatingtile_union.Rotation;
                return new SuccessResult();
            }
            return new SuccessResult();
        }
    }
}
