using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TileBuilder.Command;
using UnityEngine;

namespace TileBuilder.Validator
{
    public interface IValidator
    {
        public TileBuilder.GameMode GameMode { get; }
        public Result ValidateCommand(ICommand command);
    }

    public class Basic : IValidator
    {
        private readonly TileBuilderImpl tileBuilder;

        public Basic(TileBuilderImpl tileBuilder)
        {
            this.tileBuilder = tileBuilder;
        }

        TileBuilder.GameMode IValidator.GameMode => throw new InvalidOperationException();

        public Result ValidateCommand(ICommand command)
        {
            return command switch
            {
                DropRoom dropRoom => tileBuilder.IsValidPlacing(dropRoom.CoreModel),
                ValidateBuilding => tileBuilder.Validate(),
                BorrowRoom borrowRoom
                    when tileBuilder.GetTileUnionInPosition(borrowRoom.BorrowingPosition) == null
                    => new FailResult("No room to borrow"),
                GrowMeetingRoom growMeeting => tileBuilder.CanGrowMeetingRoom(growMeeting.MeetingRoom),
                _ => new SuccessResult()
            };
        }
    }

    public class BuildMode : IValidator
    {
        private readonly TileBuilderImpl tileBuilder;
        private readonly Basic basic;

        public BuildMode(TileBuilderImpl tileBuilder)
        {
            this.tileBuilder = tileBuilder;
            basic = new Basic(tileBuilder);
        }

        TileBuilder.GameMode IValidator.GameMode => TileBuilder.GameMode.Build;

        public Result ValidateCommand(ICommand command)
        {
            Result baseResult = basic.ValidateCommand(command);
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
                IEnumerable<Vector2Int> newPositions = tileBuilder.InstantiatedViews[
                    showRoomIllusion.CoreModel.Uid
                ].GetImaginePlaces(showRoomIllusion.CoreModel.TileUnionModel.PlacingProperties);
                return newPositions.All(x => tileBuilder.GetAllInsidePositions().Contains(x))
                    ? new SuccessResult()
                    : new FailResult("Can not show in outside");
            }
            if (command is DropRoom dropRoom)
            {
                IEnumerable<Vector2Int> newPositions = tileBuilder.InstantiatedViews[
                    dropRoom.CoreModel.Uid
                ].GetImaginePlaces(dropRoom.CoreModel.TileUnionModel.PlacingProperties);
                return
                    tileBuilder
                        .GetTileUnionsInPositions(newPositions)
                        .All(x => x.IsAllWithMark("Freespace"))
                    && newPositions.All(x => tileBuilder.GetAllInsidePositions().Contains(x))
                    ? new SuccessResult()
                    : new FailResult("Can not place on another room");
            }
            return
                command is BorrowRoom borrowRoom
                && tileBuilder.GetTileUnionInPosition(borrowRoom.BorrowingPosition) != null
                ? (
                    tileBuilder
                        .GetTileUnionInPosition(borrowRoom.BorrowingPosition)
                        .IsAllWithMark("Immutable"),
                    tileBuilder
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

    public class GameMode : IValidator
    {
        public GameMode() { }

        TileBuilder.GameMode IValidator.GameMode => TileBuilder.GameMode.Play;

        public Result ValidateCommand(ICommand command)
        {
            return new FailResult("Cannot do anything in Game Mode");
        }
    }

    public class GodMode : IValidator
    {
        private readonly TileBuilderImpl tileBuilder;
        private readonly Basic basic;

        public GodMode(TileBuilderImpl tileBuilder)
        {
            this.tileBuilder = tileBuilder;
            basic = new Basic(tileBuilder);
        }

        TileBuilder.GameMode IValidator.GameMode => TileBuilder.GameMode.God;

        public Result ValidateCommand(ICommand command)
        {
            Result baseResult = basic.ValidateCommand(command);
            if (baseResult.Failure)
            {
                return baseResult;
            }
            if (command is DropRoom dropRoom)
            {
                IEnumerable<Vector2Int> newPositions = tileBuilder.InstantiatedViews[
                    dropRoom.CoreModel.Uid
                ].GetImaginePlaces(dropRoom.CoreModel.TileUnionModel.PlacingProperties);
                return
                    tileBuilder
                        .GetTileUnionsInPositions(newPositions)
                        .All(x => x.IsAllWithMark("Freespace"))
                    || newPositions.Intersect(tileBuilder.GetAllPositions()).Count() == 0
                    ? new SuccessResult()
                    : new FailResult("Can not place on another room");
            }
            return new SuccessResult();
        }
    }
}
