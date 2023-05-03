using Common;
using System;
using UnityEngine;

public class MoveSelectedTileToRayCommand : ICommand
{
    TileBuilder tileBuilder;
    Ray ray;
    public MoveSelectedTileToRayCommand(TileBuilder tileBuilder, Ray ray)
    {
        this.tileBuilder = tileBuilder;
        this.ray = ray;
    }
    public Result Execute()
    {
        var result = tileBuilder.BuilderMatrix.GetMatrixPosition(ray);
        if (result.Success)
        {
            Vector2Int point = (result as Result<Vector2Int>).Data;
            Direction direction;
            var delta = point - tileBuilder.SelectedTile.CenterTilePosition;
            if (Math.Abs(delta.x) == Math.Abs(delta.y) && Math.Abs(delta.y) == 0)
            {
                return new FailResult("Zero moving");
            }
            else if (Math.Abs(delta.x) > Math.Abs(delta.y))
            {
                direction = delta.x > 0 ? Direction.Right : Direction.Left;
            }
            else if (Math.Abs(delta.x) < Math.Abs(delta.y))
            {
                direction = delta.y > 0 ? Direction.Up : Direction.Down;
            }
            else
            {
                if (UnityEngine.Random.value > 0.5)
                {
                    direction = delta.x > 0 ? Direction.Right : Direction.Left;
                }
                else
                {
                    direction = delta.y > 0 ? Direction.Up : Direction.Down;
                }
            }
            var command = new MoveSelectedTileCommand(tileBuilder, direction);
            return tileBuilder.Execute(command);
        }
        else
        {
            return new FailResult("No plane hits");
        }
    }
}

