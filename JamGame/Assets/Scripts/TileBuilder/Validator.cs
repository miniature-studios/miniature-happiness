using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using TileBuilder.Command;
using TileUnion;
using UnityEngine;
using static TileUnion.MeetingRoomLogics;

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
                GrowMeetingRoom growMeeting => ValidateGrowMeetingRoom(growMeeting),
                _ => new SuccessResult()
            };
        }

        public Result ValidateGrowMeetingRoom(GrowMeetingRoom command)
        {
            if (command.GrowthCount == 0)
            {
                return new SuccessResult();
            }

            MeetingRoomGrowingInformation meetingRoomGrowingInformation =
                command.MeetingRoom.GetMeetingRoomGrowingInformation(command.GrowthCount);
            foreach (Vector2Int position in meetingRoomGrowingInformation.PositionsToTake)
            {
                TileUnionImpl targetTileUnion = tileBuilder.GetTileUnionInPosition(position);
                if (
                    targetTileUnion
                        .GetAllUniqueMarks()
                        .Intersect(command.MeetingRoom.IncorrectMarks)
                        .Count() > 0
                )
                {
                    return new FailResult("Cannot borrow tile with incorrect marks.");
                }
            }
            return new SuccessResult();
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
            if (command is GrowMeetingRoom)
            {
                return new SuccessResult();
            }
            if (
                command is BorrowRoom borrowRoom
                && tileBuilder.GetTileUnionInPosition(borrowRoom.BorrowingPosition) != null
            )
            {
                bool borrowRoomImmutable = tileBuilder
                    .GetTileUnionInPosition(borrowRoom.BorrowingPosition)
                    .IsAllWithMark("Immutable");

                bool borrowRoomFreespace = tileBuilder
                    .GetTileUnionInPosition(borrowRoom.BorrowingPosition)
                    .IsAllWithMark("Freespace");

                Result result = (borrowRoomImmutable, borrowRoomFreespace) switch
                {
                    (true, _) => new FailResult("Immutable Tile"),
                    (_, true) => new FailResult("Free space Tile"),
                    _ => new SuccessResult()
                };

                return result;
            }

            return new FailResult("Cannot do this command");
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
