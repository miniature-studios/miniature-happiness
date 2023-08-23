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

    public abstract class BaseValidator : IValidator
    {
        protected readonly TileBuilderImpl TileBuilder;

        public BaseValidator(TileBuilderImpl tileBuilder)
        {
            TileBuilder = tileBuilder;
        }

        public abstract Result ValidateCommand(ICommand command);

        public Result BaseValidateCommand(ICommand command)
        {
            return command is DropRoom dropRoom
                ? TileBuilder.IsValidPlacing(dropRoom.CoreModel)
                : command is ValidateBuilding
                    ? TileBuilder.Validate()
                    : new SuccessResult();
        }
    }

    public class BuildMode : BaseValidator
    {
        public BuildMode(TileBuilderImpl tileBuilder)
            : base(tileBuilder) { }

        public override Result ValidateCommand(ICommand command)
        {
            Result baseResult = BaseValidateCommand(command);
            if (baseResult.Failure)
            {
                return baseResult;
            }
            if (command is ValidateBuilding or HideSelectedRoom)
            {
                return new SuccessResult();
            }
            if (command is ShowSelectedRoom showRoomIllusion)
            {
                IEnumerable<Vector2Int> newPositions = TileBuilder.InstantiatedViews[
                    showRoomIllusion.CoreModel.HashCode
                ].GetImaginePlaces(showRoomIllusion.CoreModel.TileUnionModel.PlacingProperties);
                return newPositions.All(x => TileBuilder.GetAllInsidePositions().Contains(x))
                    ? new SuccessResult()
                    : new FailResult("Can not show in outside");
            }
            if (command is DropRoom dropRoom)
            {
                IEnumerable<Vector2Int> newPositions = TileBuilder.InstantiatedViews[
                    dropRoom.CoreModel.HashCode
                ].GetImaginePlaces(dropRoom.CoreModel.TileUnionModel.PlacingProperties);
                return
                    TileBuilder
                        .GetTileUnionsInPositions(newPositions)
                        .All(x => x.IsAllWithMark("Freespace"))
                    && newPositions.All(x => TileBuilder.GetAllInsidePositions().Contains(x))
                    ? new SuccessResult()
                    : new FailResult("Can not place on another room");
            }
            return command is BorrowRoom borrowRoom
                ? (
                    TileBuilder
                        .GetTileUnionInPosition(borrowRoom.BorrowingPosition)
                        .IsAllWithMark("Immutable"),
                    TileBuilder
                        .GetTileUnionInPosition(borrowRoom.BorrowingPosition)
                        .IsAllWithMark("Freespace")
                ) switch
                {
                    (true, _) => new FailResult("Immutable Tile"),
                    (_, true) => new FailResult("Free space Tile"),
                    _ => new SuccessResult()
                }
                : new FailResult("Cannot do this command");
        }
    }

    public class GameMode : BaseValidator
    {
        public GameMode(TileBuilderImpl tileBuilder)
            : base(tileBuilder) { }

        public override Result ValidateCommand(ICommand command)
        {
            return new FailResult("Cannot do anything in Game Mode");
        }
    }

    public class GodMode : BaseValidator
    {
        public GodMode(TileBuilderImpl tileBuilder)
            : base(tileBuilder) { }

        public override Result ValidateCommand(ICommand command)
        {
            Result baseResult = BaseValidateCommand(command);
            if (baseResult.Failure)
            {
                return baseResult;
            }
            if (command is DropRoom dropRoom)
            {
                IEnumerable<Vector2Int> newPositions = TileBuilder.InstantiatedViews[
                    dropRoom.CoreModel.HashCode
                ].GetImaginePlaces(dropRoom.CoreModel.TileUnionModel.PlacingProperties);
                return
                    TileBuilder
                        .GetTileUnionsInPositions(newPositions)
                        .All(x => x.IsAllWithMark("Freespace"))
                    || newPositions.Intersect(TileBuilder.GetAllPositions()).Count() == 0
                    ? new SuccessResult()
                    : new FailResult("Can not place on another room");
            }
            return new SuccessResult();
        }
    }
}
