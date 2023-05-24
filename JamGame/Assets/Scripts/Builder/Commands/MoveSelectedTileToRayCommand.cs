using Common;
using System;
using UnityEngine;

public class MoveSelectedTileToRayCommand : ICommand
{
    private Ray ray;
    public MoveSelectedTileToRayCommand(Ray _ray)
    {
        ray = _ray;
    }
    public Result Execute(TileBuilderController tile_builder_controller)
    {
        Result<Vector2Int> result = tile_builder_controller.TileBuilder.BuilderMatrix.GetMatrixPosition(ray);
        if (result.Success)
        {
            Vector2Int point = result.Data;
            Direction direction;
            Vector2Int delta = point - tile_builder_controller.TileBuilder.SelectedTile.CenterPosition;
            if (Math.Abs(delta.x) == Math.Abs(delta.y) && Math.Abs(delta.y) == 0)
            {
                return new FailResult("Zero moving");
            }
            else
            {
                direction = (Math.Abs(delta.x) > Math.Abs(delta.y), delta.x >= 0, delta.y >= 0) switch
                {
                    (true, true, _) => Direction.Right,
                    (true, false, _) => Direction.Left,
                    (false, _, true) => Direction.Up,
                    (false, _, false) => Direction.Down
                };
            }
            MoveSelectedTileCommand command = new(direction);
            return tile_builder_controller.Execute(command);
        }
        else
        {
            return new FailResult("No plane hits");
        }
    }
}

