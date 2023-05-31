using Common;
using System;
using UnityEngine;

public class MoveSelectedTileCommand : ICommand
{
    public Direction? Direction { get; }
    public MoveSelectedTileCommand(Direction direction)
    {
        Direction = direction;
    }
    public MoveSelectedTileCommand(Ray ray, BuilderMatrix builder_matrix, Vector2Int? selected_tile_position)
    {
        Result<Vector2Int> result = builder_matrix.GetMatrixPosition(ray);
        if (selected_tile_position != null && result.Success)
        {
            Vector2Int point = result.Data;
            Vector2Int delta = point - selected_tile_position.Value;
            Direction = Math.Abs(delta.x) == Math.Abs(delta.y) && Math.Abs(delta.y) == 0
                ? null
                : (Math.Abs(delta.x) > Math.Abs(delta.y), delta.x >= 0, delta.y >= 0) switch
                {
                    (true, true, _) => Common.Direction.Right,
                    (true, false, _) => Common.Direction.Left,
                    (false, _, true) => Common.Direction.Up,
                    (false, _, false) => Common.Direction.Down
                };
        }
        else
        {
            Direction = null;
        }
    }
    public Result Execute(TileBuilder tile_builder)
    {
        if (Direction == null)
        {
            return new FailResult("Null diraction");
        }
        return tile_builder.MoveSelectedTile((Direction)Direction);
    }
}