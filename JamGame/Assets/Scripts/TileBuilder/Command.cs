using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TileBuilder.Command
{
    public interface ICommand
    {
        public Result Execute(TileBuilder tile_builder);
    }

    public class AddTileToScene : ICommand
    {
        public TileUnion TilePrefab;
        public Vector2Int CreatingPosition;
        public int CreatingRotation;
        public Ray Ray;

        public AddTileToScene(TileUnion tile_prefab, Ray ray)
        {
            TilePrefab = tile_prefab;
            CreatingPosition = new();
            CreatingRotation = 0;
            Ray = ray;
        }

        public AddTileToScene(TileUnion tile_prefab)
        {
            TilePrefab = tile_prefab;
            CreatingPosition = new();
            CreatingRotation = 0;
        }

        public Result Execute(TileBuilder tile_builder)
        {
            return tile_builder.AddTileIntoBuilding(TilePrefab, CreatingPosition, CreatingRotation);
        }
    }

    public class CompletePlacing : ICommand
    {
        public Result Execute(TileBuilder tile_builder)
        {
            return tile_builder.ComletePlacing();
        }
    }

    public class DeleteSelectedTile : ICommand
    {
        private RoomInventoryUI tileUIPrefab;
        private readonly Action<RoomInventoryUI> sendUIPrefab;

        public DeleteSelectedTile(Action<RoomInventoryUI> send_ui_prefab)
        {
            sendUIPrefab = send_ui_prefab;
        }

        public Result Execute(TileBuilder tile_builder)
        {
            Result response = tile_builder.DeleteSelectedTile(out tileUIPrefab);
            sendUIPrefab(tileUIPrefab);
            return response;
        }
    }

    public class MoveSelectedTile : ICommand
    {
        public Direction? Direction { get; }

        public MoveSelectedTile(Direction direction)
        {
            Direction = direction;
        }

        public MoveSelectedTile(
            Ray ray,
            BuilderMatrix builder_matrix,
            Vector2Int? selected_tile_position
        )
        {
            Result<Vector2Int> result = builder_matrix.GetMatrixPosition(ray);
            if (selected_tile_position != null && result.Success)
            {
                Vector2Int point = result.Data;
                Vector2Int delta = point - selected_tile_position.Value;
                Direction =
                    Math.Abs(delta.x) == Math.Abs(delta.y) && Math.Abs(delta.y) == 0
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
            return Direction == null
                ? new FailResult("Null diraction")
                : tile_builder.MoveSelectedTile((Direction)Direction);
        }
    }

    public class RotateSelectedTile : ICommand
    {
        public RotationDirection Direction { get; }

        public RotateSelectedTile(RotationDirection direction)
        {
            Direction = direction;
        }

        public Result Execute(TileBuilder tile_builder)
        {
            return tile_builder.RotateSelectedTile(Direction);
        }
    }

    public class SelectTile : ICommand
    {
        public TileUnion Tile;

        public SelectTile(Ray ray)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, float.PositiveInfinity);
            IEnumerable<TileUnion> tiles = hits.ToList()
                .Where(x => x.collider.GetComponentInParent<TileUnion>() != null)
                .Select(x => x.collider.GetComponentInParent<TileUnion>());
            Tile = tiles.Count() != 0 ? tiles.First() : null;
        }

        public Result Execute(TileBuilder tile_builder)
        {
            return Tile == null ? new FailResult("No hits") : tile_builder.SelectTile(Tile);
        }
    }

    public class ValidateBuilding : ICommand
    {
        public Result Execute(TileBuilder tile_builder)
        {
            return tile_builder.Validate();
        }
    }
}
