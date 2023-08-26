using Level.Room;
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
            tileBuilder.DropTileUnion(CoreModel);
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
            tileBuilder.BorrowTileUnion(BorrowingPosition, out borrowedRoom);
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
}
