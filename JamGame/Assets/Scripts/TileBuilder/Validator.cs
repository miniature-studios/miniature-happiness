using Common;
using System.Collections.Generic;
using System.Linq;
using TileBuilder.Command;
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
        private GodMode godModeValidator;

        public BuildMode(TileBuilderImpl tileBuilder)
        {
            this.tileBuilder = tileBuilder;
            godModeValidator = new();
        }

        public Result ValidateCommand(ICommand command)
        {
            Result validatorResult = godModeValidator.ValidateCommand(command);
            if (validatorResult.Failure)
            {
                return validatorResult;
            }
            if (command is ValidateBuilding)
            {
                return new SuccessResult();
            }
            if (command is ShowRoomIllusion showRoomIllusion)
            {
                IEnumerable<Vector2Int> newPositions = tileBuilder.InstantiatedViews[
                    showRoomIllusion.CoreModel.HashCode
                ].GetImaginePlaces(showRoomIllusion.Position.Value, showRoomIllusion.Rotation);
                return newPositions.All(x => tileBuilder.GetAllInsideListPositions().Contains(x))
                    ? new SuccessResult()
                    : new FailResult("Can not show in outside");
            }
            if (command is DropRoom dropRoom)
            {
                IEnumerable<Vector2Int> newPositions = tileBuilder.InstantiatedViews[
                    dropRoom.CoreModel.HashCode
                ].GetImaginePlaces(dropRoom.Position.Value, dropRoom.Rotation);
                return
                    tileBuilder
                        .GetTileUnionsInPositions(newPositions)
                        .All(x => x.IsAllWithMark("Freespace"))
                    && newPositions.All(x => tileBuilder.GetAllInsideListPositions().Contains(x))
                    ? new SuccessResult()
                    : new FailResult("Can not place on other room");
            }
            return command is BorrowRoom borrowRoom
                ? (
                    borrowRoom.TileUnionImpl.IsAllWithMark("Immutable"),
                    borrowRoom.TileUnionImpl.IsAllWithMark("Freespace")
                ) switch
                {
                    (true, _) => new FailResult("Immutable Tile"),
                    (_, true) => new FailResult("Free space Tile"),
                    _ => new SuccessResult()
                }
                : (Result)new FailResult("Can not do this command");
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
        public Result ValidateCommand(ICommand command)
        {
            return command switch
            {
                DropRoom dropRoom when dropRoom.Position == null => new FailResult("No hits"),
                ShowRoomIllusion showRoomIllusion when showRoomIllusion.Position == null
                    => new FailResult("No hits"),
                BorrowRoom borrowRoom when borrowRoom.TileUnionImpl == null
                    => new FailResult("No hits"),
                _ => new SuccessResult()
            };
        }
    }
}
