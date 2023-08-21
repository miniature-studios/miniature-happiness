using Common;
using Level.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using TileUnion;
using UnityEngine;

namespace TileBuilder.Command
{
    public interface ICommand
    {
        public Result Execute(TileBuilderImpl tileBuilder);
    }

    public class ValidateBuilding : ICommand
    {
        public Result Execute(TileBuilderImpl tileBuilder)
        {
            return tileBuilder.Validate();
        }
    }

    /// <summary>
    /// Places room directly into building by ray
    /// </summary>
    public class DropRoom : ICommand
    {
        public Vector2Int? Position { get; private set; }
        public int Rotation { get; private set; }
        public CoreModel CoreModel { get; private set; }

        public DropRoom(CoreModel coreModel, Vector2Int position, int rotation)
        {
            CoreModel = coreModel;
            Position = position;
            Rotation = rotation;
        }

        public DropRoom(CoreModel coreModel, Ray ray, int rotation, Matrix builderMatrix)
        {
            CoreModel = coreModel;
            Result<Vector2Int> result = builderMatrix.GetMatrixPosition(ray);
            Position = result.Success ? result.Data : null;
            Rotation = rotation;
        }

        public Result Execute(TileBuilderImpl tileBuilder)
        {
            return tileBuilder.DropTileUnion(CoreModel, Position.Value, Rotation);
        }
    }

    /// <summary>
    /// Borrows room from building by ray
    /// </summary>
    public class BorrowRoom : ICommand
    {
        public TileUnionImpl TileUnionImpl { get; private set; }
        public Action<CoreModel> BorrowedModel { get; private set; }

        public BorrowRoom(Ray ray, Action<CoreModel> borrowedModel)
        {
            BorrowedModel = borrowedModel;
            RaycastHit[] hits = Physics.RaycastAll(ray, float.PositiveInfinity);
            IEnumerable<TileUnionImpl> tiles = hits.ToList()
                .Where(x => x.collider.GetComponentInParent<TileUnionImpl>() != null)
                .Select(x => x.collider.GetComponentInParent<TileUnionImpl>());
            TileUnionImpl = tiles.Count() != 0 ? tiles.First() : null;
        }

        public Result Execute(TileBuilderImpl tileBuilder)
        {
            return tileBuilder.BorrowTileUnion(TileUnionImpl, BorrowedModel);
        }
    }

    /// <summary>
    /// Shows selecting room in building
    /// </summary>
    public class ShowRoomIllusion : ICommand
    {
        public Vector2Int? Position { get; private set; }
        public int Rotation { get; private set; }
        public CoreModel CoreModel { get; private set; }

        public ShowRoomIllusion(CoreModel coreModel, Ray ray, int rotation, Matrix builderMatrix)
        {
            CoreModel = coreModel;
            Result<Vector2Int> result = builderMatrix.GetMatrixPosition(ray);
            Position = result.Success ? result.Data : null;
            Rotation = rotation;
        }

        public Result Execute(TileBuilderImpl tileBuilder)
        {
            return tileBuilder.ShowTileUnionIllusion(CoreModel, Position.Value, Rotation);
        }
    }
}
