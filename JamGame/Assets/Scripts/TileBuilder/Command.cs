using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TileUnion;
using UnityEngine;

namespace TileBuilder.Command
{
    public interface ICommand
    {
        public Result Execute(TileBuilderImpl tile_builder, ref TileUnionImpl selected_tile);
    }

    public class AddTileToScene : ICommand
    {
        public TileUnionImpl TilePrefab;
        public Vector2Int CreatingPosition;
        public int CreatingRotation;
        public Ray Ray;

        public AddTileToScene(TileUnionImpl tile_prefab, Ray ray)
        {
            TilePrefab = tile_prefab;
            CreatingPosition = new();
            CreatingRotation = 0;
            Ray = ray;
        }

        public AddTileToScene(TileUnionImpl tile_prefab)
        {
            TilePrefab = tile_prefab;
            CreatingPosition = new();
            CreatingRotation = 0;
        }

        public Result Execute(TileBuilderImpl tile_builder, ref TileUnionImpl selected_tile)
        {
            return tile_builder.AddTileIntoBuilding(
                TilePrefab,
                ref selected_tile,
                CreatingPosition,
                CreatingRotation
            );
        }
    }

    public class CompletePlacing : ICommand
    {
        public Result Execute(TileBuilderImpl tile_builder, ref TileUnionImpl selected_tile)
        {
            return tile_builder.ComletePlacing(ref selected_tile);
        }
    }

    public class DeleteSelectedTile : ICommand
    {
        private Level.Inventory.Room.Model tileUIPrefab;
        private Action<Level.Inventory.Room.Model> sendUIPrefab;

        public DeleteSelectedTile(Action<Level.Inventory.Room.Model> send_ui_prefab)
        {
            sendUIPrefab = send_ui_prefab;
        }

        public Result Execute(TileBuilderImpl tile_builder, ref TileUnionImpl selected_tile)
        {
            Result response = tile_builder.DeleteSelectedTile(out tileUIPrefab, ref selected_tile);
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

        public MoveSelectedTile(Ray ray, Matrix builder_matrix, Vector2Int? selected_tile_position)
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

        public Result Execute(TileBuilderImpl tile_builder, ref TileUnionImpl selected_tile)
        {
            return tile_builder.MoveSelectedTile((Direction)Direction, ref selected_tile);
        }
    }

    public class RotateSelectedTile : ICommand
    {
        public RotationDirection Direction { get; }

        public RotateSelectedTile(RotationDirection direction)
        {
            Direction = direction;
        }

        public Result Execute(TileBuilderImpl tile_builder, ref TileUnionImpl selected_tile)
        {
            return tile_builder.RotateSelectedTile(Direction, ref selected_tile);
        }
    }

    public class SelectTile : ICommand
    {
        public TileUnionImpl Tile;

        public SelectTile(Ray ray)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, float.PositiveInfinity);
            IEnumerable<TileUnionImpl> tiles = hits.ToList()
                .Where(x => x.collider.GetComponentInParent<TileUnionImpl>() != null)
                .Select(x => x.collider.GetComponentInParent<TileUnionImpl>());
            Tile = tiles.Count() != 0 ? tiles.First() : null;
        }

        public Result Execute(TileBuilderImpl tile_builder, ref TileUnionImpl selected_tile)
        {
            return tile_builder.SelectTile(Tile, ref selected_tile);
        }
    }

    public class ValidateBuilding : ICommand
    {
        public Result Execute(TileBuilderImpl tile_builder, ref TileUnionImpl selected_tile)
        {
            return tile_builder.Validate();
        }
    }
}
