using Level.Room;
using System.Collections.Generic;
using System.Collections.Immutable;
using TileUnion;
using UnityEngine;

namespace TileBuilder.Command
{
    public interface ICommand
    {
        public void Execute(TileBuilderImpl tileBuilder);
    }

    public class ValidateBuilding : ICommand
    {
        public void Execute(TileBuilderImpl tileBuilder) { }
    }

    public class DropRoom : ICommand
    {
        public CoreModel CoreModel { get; private set; }

        public DropRoom(CoreModel coreModel)
        {
            CoreModel = coreModel;
        }

        public void Execute(TileBuilderImpl tileBuilder)
        {
            tileBuilder.InstantiateTileUnion(CoreModel);
        }
    }

    public class BorrowRoom : ICommand
    {
        public Vector2Int BorrowingPosition { get; private set; }
        private CoreModel borrowedRoom = null;
        public CoreModel BorrowedRoom => borrowedRoom;

        public BorrowRoom(Vector2Int borrowingPosition)
        {
            BorrowingPosition = borrowingPosition;
        }

        public void Execute(TileBuilderImpl tileBuilder)
        {
            borrowedRoom = tileBuilder.RemoveTileUnion(BorrowingPosition);
        }
    }

    public class ShowSelectedRoom : ICommand
    {
        public CoreModel CoreModel { get; private set; }

        public ShowSelectedRoom(CoreModel coreModel)
        {
            CoreModel = coreModel;
        }

        public void Execute(TileBuilderImpl tileBuilder)
        {
            tileBuilder.ShowSelectedTileUnion(CoreModel);
        }
    }

    public class HideSelectedRoom : ICommand
    {
        public void Execute(TileBuilderImpl tileBuilder)
        {
            tileBuilder.ResetStashedViews();
        }
    }

    public class RemoveAllRooms : ICommand
    {
        public void Execute(TileBuilderImpl tileBuilder)
        {
            tileBuilder.DeleteAllTiles();
        }
    }

    public class GrowMeetingRoom : ICommand
    {
        public MeetingRoomLogics MeetingRoom { get; private set; }

        public int GrowthCount { get; private set; }

        private List<CoreModel> borrowedCoreModels = new();
        public ImmutableList<CoreModel> BorrowedCoreModels => borrowedCoreModels.ToImmutableList();

        public GrowMeetingRoom(MeetingRoomLogics meetingRoom, int growthCount)
        {
            MeetingRoom = meetingRoom;
            GrowthCount = growthCount;
        }

        public void Execute(TileBuilderImpl tileBuilder)
        {
            if (GrowthCount == 0)
            {
                return;
            }

            MeetingRoomLogics.MeetingRoomGrowingInformation growingInfo =
                MeetingRoom.GetMeetingRoomGrowingInformation(GrowthCount);
            MeetingRoom.AddTiles(growingInfo);

            foreach (Vector2Int pos in growingInfo.PositionsToTake)
            {
                CoreModel coreModel = tileBuilder.RemoveTileUnion(pos);
                if (coreModel != null)
                {
                    borrowedCoreModels.Add(coreModel);
                }
            }

            TileUnionImpl meetingRoom = tileBuilder.BorrowTileUnion(MeetingRoom.TileUnion.Position);

            tileBuilder.PlaceTileUnion(meetingRoom);
        }
    }
}
