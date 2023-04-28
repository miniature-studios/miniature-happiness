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
    public Response Execute()
    {
        Plane plane = new(Vector3.up, new Vector3(0, BuilderMatrix.SelectingPlaneHeight, 0));
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector2Int point = BuilderMatrix.GetMatrixPosition(new(hitPoint.x, hitPoint.z));

            Direction direction;
            var delta = point - tileBuilder.SelectedTile.CenterTilePosition;
            if (Math.Abs(delta.x) == Math.Abs(delta.y) && Math.Abs(delta.y) == 0)
            {
                return new Response("Zero moving", false);
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
            return new Response("No plane hits", false);
        }
    }
}

