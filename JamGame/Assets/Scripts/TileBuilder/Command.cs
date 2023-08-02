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
        public Result Execute(TileBuilderImpl tile_builder);
    }

    public class AddTileToScene : ICommand
    {
        public TileUnionImpl TilePrefab;
        public Vector2Int CreatingPosition;
        public int CreatingRotation;
        public Ray Ray;
        private SelectedTileWrapper selectedTileWrapper;

        public AddTileToScene(
            TileUnionImpl tile_prefab,
            Ray ray,
            SelectedTileWrapper selected_tile_wrapper
        )
        {
            TilePrefab = tile_prefab;
            CreatingPosition = new();
            CreatingRotation = 0;
            Ray = ray;
            selectedTileWrapper = selected_tile_wrapper;
        }

        public AddTileToScene(TileUnionImpl tile_prefab, SelectedTileWrapper selected_tile_wrapper)
        {
            TilePrefab = tile_prefab;
            CreatingPosition = new();
            CreatingRotation = 0;
            selectedTileWrapper = selected_tile_wrapper;
        }

        public Result Execute(TileBuilderImpl tile_builder)
        {
            return tile_builder.AddTileIntoBuilding(
                TilePrefab,
                selectedTileWrapper,
                CreatingPosition,
                CreatingRotation
            );
        }
    }

    public class CompletePlacing : ICommand
    {
        private SelectedTileWrapper selectedTileWrapper;

        public CompletePlacing(SelectedTileWrapper selected_tile_wrapper)
        {
            selectedTileWrapper = selected_tile_wrapper;
        }

        public Result Execute(TileBuilderImpl tile_builder)
        {
            return selectedTileWrapper.Value == null
                ? new SuccessResult()
                : tile_builder.CompletePlacing(selectedTileWrapper);
        }
    }

    public class DeleteSelectedTile : ICommand
    {
        private Level.Inventory.Room.Model tileUIPrefab;
        private Action<Level.Inventory.Room.Model> sendUIPrefab;
        private SelectedTileWrapper selectedTileWrapper;

        public DeleteSelectedTile(
            Action<Level.Inventory.Room.Model> send_ui_prefab,
            SelectedTileWrapper selected_tile_wrapper
        )
        {
            sendUIPrefab = send_ui_prefab;
            selectedTileWrapper = selected_tile_wrapper;
        }

        public Result Execute(TileBuilderImpl tile_builder)
        {
            if (selectedTileWrapper.Value == null)
            {
                return new SuccessResult();
            }
            Result response = tile_builder.DeleteSelectedTile(
                out tileUIPrefab,
                selectedTileWrapper
            );
            sendUIPrefab(tileUIPrefab);
            return response;
        }
    }

    public class MoveSelectedTile : ICommand
    {
        public Direction? Direction { get; }
        private SelectedTileWrapper selectedTileWrapper;

        public MoveSelectedTile(Direction direction, SelectedTileWrapper selected_tile_wrapper)
        {
            Direction = direction;
            selectedTileWrapper = selected_tile_wrapper;
        }

        public MoveSelectedTile(
            Ray ray,
            Matrix builder_matrix,
            Vector2Int? selected_tile_position,
            SelectedTileWrapper selected_tile_wrapper
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
            selectedTileWrapper = selected_tile_wrapper;
        }

        public Result Execute(TileBuilderImpl tile_builder)
        {
            return tile_builder.MoveSelectedTile((Direction)Direction, selectedTileWrapper);
        }
    }

    public class RotateSelectedTile : ICommand
    {
        public RotationDirection Direction { get; }

        private SelectedTileWrapper selectedTileWrapper;

        public RotateSelectedTile(
            RotationDirection direction,
            SelectedTileWrapper selected_tile_wrapper
        )
        {
            Direction = direction;
            selectedTileWrapper = selected_tile_wrapper;
        }

        public Result Execute(TileBuilderImpl tile_builder)
        {
            return tile_builder.RotateSelectedTile(Direction, selectedTileWrapper);
        }
    }

    public class SelectTile : ICommand
    {
        public TileUnionImpl Tile;
        private SelectedTileWrapper selectedTileWrapper;

        public SelectTile(Ray ray, SelectedTileWrapper selected_tile_wrapper)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, float.PositiveInfinity);
            IEnumerable<TileUnionImpl> tiles = hits.ToList()
                .Where(x => x.collider.GetComponentInParent<TileUnionImpl>() != null)
                .Select(x => x.collider.GetComponentInParent<TileUnionImpl>());
            Tile = tiles.Count() != 0 ? tiles.First() : null;
            selectedTileWrapper = selected_tile_wrapper;
        }

        public Result Execute(TileBuilderImpl tile_builder)
        {
            return tile_builder.SelectTile(Tile, selectedTileWrapper);
        }
    }

    public class ValidateBuilding : ICommand
    {
        public Result Execute(TileBuilderImpl tile_builder)
        {
            return tile_builder.Validate();
        }
    }
}
