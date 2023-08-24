using Level.Room;
using System;
using System.Collections.Generic;
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
        public List<Action<CoreModel>> GetBorrowedRoom { get; private set; } = new();

        public BorrowRoom(Vector2Int borrowingPosition, Action<CoreModel> getBorrowedRoom)
        {
            BorrowingPosition = borrowingPosition;
            GetBorrowedRoom.Add(getBorrowedRoom);
        }

        public void Execute(TileBuilderImpl tileBuilder)
        {
            tileBuilder.BorrowTileUnion(
                BorrowingPosition,
                (coreModel) => GetBorrowedRoom.ForEach(x => x.Invoke(coreModel))
            );
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
