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
        public Result ValidateCommand(ICommand command);
    }

    public class BuildMode : IValidator
    {
        private readonly TileBuilderImpl tileBuilder;
        private readonly SelectedTileWrapper selectedTileCover;
        private GodMode godModeValidator;

        public BuildMode(TileBuilderImpl tileBuilder, SelectedTileWrapper selectedTileCover)
        {
            this.tileBuilder = tileBuilder;
            godModeValidator = new(tileBuilder, selectedTileCover);
            this.selectedTileCover = selectedTileCover;
        }

        public Result ValidateCommand(ICommand command)
        {
            Result validator_result = godModeValidator.ValidateCommand(command);
            if (validator_result.Failure)
            {
                return validator_result;
            }
            if (command is CompletePlacing or DeleteSelectedTile or ValidateBuilding)
            {
                return new SuccessResult();
            }
            if (command is AddTileToScene add_command)
            {
                TileUnionImpl creating_tile_union =
                    add_command.TilePrefab.GetComponent<TileUnionImpl>();
                IEnumerable<Vector2Int> inside_list_positions =
                    tileBuilder.GetFreeSpaceInsideListPositions();

                Vector2Int picked_position;
                Result<Vector2Int> result = tileBuilder.BuilderMatrix.GetMatrixPosition(
                    add_command.Ray
                );
                picked_position = result.Success ? result.Data : Vector2Int.zero;

                int rotation = 0;
                bool chose = false;
                Vector2Int buffer_position = Vector2Int.zero;
                int buffer_rotation = 0;
                float buffer_distance = float.MaxValue;

                while (rotation < 4)
                {
                    foreach (Vector2Int free_position in inside_list_positions)
                    {
                        IEnumerable<Vector2Int> future_places = creating_tile_union.GetImaginePlaces(
                            free_position,
                            creating_tile_union.Rotation + rotation
                        );
                        if (
                            inside_list_positions.Intersect(future_places).Count()
                            == creating_tile_union.TilesCount
                        )
                        {
                            chose = true;
                            float calc_distance = Vector2.Distance(
                                CenterOfMassTools.GetCenterOfMass(future_places.ToList()),
                                picked_position
                            );
                            if (calc_distance < buffer_distance)
                            {
                                buffer_position = free_position;
                                buffer_rotation = rotation;
                                buffer_distance = calc_distance;
                            }
                        }
                    }
                    rotation++;
                }
                if (chose)
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
                    select_command.Tile.IsAllWithMark("Immutable"),
                    select_command.Tile.IsAllWithMark("Freespace")
                ) switch
                {
                    (true, _) => new FailResult("Immutable Tile"),
                    (_, true) => new FailResult("Free space Tile"),
                    _ => new SuccessResult()
                };
            }
            if (command is MoveSelectedTile move_command)
            {
                Vector2Int new_union_position =
                    selectedTileCover.Value.Position + move_command.Direction.Value.ToVector2Int();
                IEnumerable<Vector2Int> newPositions = selectedTileCover.Value.GetImaginePlaces(
                    new_union_position,
                    selectedTileCover.Value.Rotation
                );
                return !tileBuilder
                    .GetTileUnionsInPositions(newPositions)
                    .All(x => !x.IsAllWithMark("Outside"))
                    ? new FailResult("Can not move outside")
                    : new SuccessResult();
            }
            if (command is RotateSelectedTile rotate_command)
            {
                IEnumerable<Vector2Int> newPosition = selectedTileCover.Value.GetImaginePlaces(
                    selectedTileCover.Value.Position,
                    selectedTileCover.Value.Rotation + (int)rotate_command.Direction
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
        public Result ValidateCommand(ICommand command)
        {
            return new FailResult("Cannot do anything in Game Mode");
        }
    }

    public class GodMode : IValidator
    {
        private readonly TileBuilderImpl tileBuilder;
        private readonly SelectedTileWrapper selectedTileCover;

        public GodMode(TileBuilderImpl tileBuilder, SelectedTileWrapper selectedTileCover)
        {
            this.tileBuilder = tileBuilder;
            this.selectedTileCover = selectedTileCover;
        }

        public Result ValidateCommand(ICommand command)
        {
            if (command is DeleteSelectedTile)
            {
                return selectedTileCover.Value == null
                    ? new FailResult("SelectedTile is Null")
                    : new SuccessResult();
            }
            if (command is ValidateBuilding)
            {
                return selectedTileCover.Value != null
                    ? new FailResult("SelectedTile is not Null")
                    : new SuccessResult();
            }
            if (command is SelectTile select_command)
            {
                return (
                    select_command.Tile == selectedTileCover.Value,
                    select_command.Tile == null
                ) switch
                {
                    (true, _) => new FailResult("Selected already selected tile"),
                    (_, true) => new FailResult("No hits"),
                    _ => new SuccessResult()
                };
            }
            if (command is MoveSelectedTile move_command)
            {
                return (selectedTileCover.Value == null, move_command.Direction == null) switch
                {
                    (true, _) => new FailResult("Not selected Tile"),
                    (_, true) => new FailResult("Null direction"),
                    _ => new SuccessResult()
                };
            }
            if (command is RotateSelectedTile)
            {
                return selectedTileCover.Value == null
                    ? new FailResult("Not selected Tile")
                    : new SuccessResult();
            }
            if (command is AddTileToScene add_command)
            {
                if (selectedTileCover.Value != null)
                {
                    return new FailResult("Complete placing previous tile");
                }
                TileUnionImpl creating_tile_union =
                    add_command.TilePrefab.GetComponent<TileUnionImpl>();
                IEnumerable<Vector2Int> inside_list_positions =
                    tileBuilder.GetFreeSpaceInsideListPositions();

                int rotation = 0;
                while (rotation < 4)
                {
                    foreach (Vector2Int free_position in inside_list_positions)
                    {
                        IEnumerable<Vector2Int> futurePlaces = creating_tile_union.GetImaginePlaces(
                            free_position,
                            creating_tile_union.Rotation + rotation
                        );
                        if (
                            inside_list_positions.Intersect(futurePlaces).Count()
                            == creating_tile_union.TilesCount
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
                            creating_tile_union.GetImaginePlaces(
                                position,
                                creating_tile_union.Rotation
                            )
                        )
                        .Count() > 0
                );
                add_command.CreatingPosition = position;
                add_command.CreatingRotation = creating_tile_union.Rotation;
                return new SuccessResult();
            }
            return new SuccessResult();
        }
    }
}
