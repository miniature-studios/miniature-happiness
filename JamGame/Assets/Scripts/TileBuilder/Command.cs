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
        private SelectedTileWrapper selectedTileCover;

        public AddTileToScene(
            TileUnionImpl tile_prefab,
            Ray ray,
            SelectedTileWrapper selected_tile_cover
        )
        {
            TilePrefab = tile_prefab;
            CreatingPosition = new();
            CreatingRotation = 0;
            Ray = ray;
            selectedTileCover = selected_tile_cover;
        }

        public AddTileToScene(TileUnionImpl tile_prefab, SelectedTileWrapper selected_tile_cover)
        {
            TilePrefab = tile_prefab;
            CreatingPosition = new();
            CreatingRotation = 0;
            selectedTileCover = selected_tile_cover;
        }

        public Result Execute(TileBuilderImpl tile_builder)
        {
            return tile_builder.AddTileIntoBuilding(
                TilePrefab,
                selectedTileCover,
                CreatingPosition,
                CreatingRotation
            );
        }
    }

    public class CompletePlacing : ICommand
    {
        private SelectedTileWrapper selectedTileCover;

        public CompletePlacing(SelectedTileWrapper selectedTileCover)
        {
            this.selectedTileCover = selectedTileCover;
        }

        public Result Execute(TileBuilderImpl tile_builder)
        {
            return selectedTileCover.Value == null
                ? new SuccessResult()
                : tile_builder.ComletePlacing(selectedTileCover);
        }
    }

    public class DeleteSelectedTile : ICommand
    {
        private Level.Inventory.Room.Model tileUIPrefab;
        private Action<Level.Inventory.Room.Model> sendUIPrefab;
        private SelectedTileWrapper selectedTileCover;

        public DeleteSelectedTile(
            Action<Level.Inventory.Room.Model> send_ui_prefab,
            SelectedTileWrapper selectedTileCover
        )
        {
            sendUIPrefab = send_ui_prefab;
            this.selectedTileCover = selectedTileCover;
        }

        public Result Execute(TileBuilderImpl tile_builder)
        {
            if (selectedTileCover.Value == null)
            {
                return new SuccessResult();
            }
            Result response = tile_builder.DeleteSelectedTile(out tileUIPrefab, selectedTileCover);
            sendUIPrefab(tileUIPrefab);
            return response;
        }
    }

    public class MoveSelectedTile : ICommand
    {
        public Direction? Direction { get; }
        private SelectedTileWrapper selectedTileCover;

        public MoveSelectedTile(Direction direction, SelectedTileWrapper selectedTileCover)
        {
            Direction = direction;
            this.selectedTileCover = selectedTileCover;
        }

        public MoveSelectedTile(
            Ray ray,
            Matrix builder_matrix,
            Vector2Int? selected_tile_position,
            SelectedTileWrapper selectedTileCover
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
            this.selectedTileCover = selectedTileCover;
        }

        public Result Execute(TileBuilderImpl tile_builder)
        {
            return tile_builder.MoveSelectedTile((Direction)Direction, selectedTileCover);
        }
    }

    public class RotateSelectedTile : ICommand
    {
        public RotationDirection Direction { get; }

        private SelectedTileWrapper selectedTileCover;

        public RotateSelectedTile(
            RotationDirection direction,
            SelectedTileWrapper selectedTileCover
        )
        {
            Direction = direction;
            this.selectedTileCover = selectedTileCover;
        }

        public Result Execute(TileBuilderImpl tile_builder)
        {
            return tile_builder.RotateSelectedTile(Direction, selectedTileCover);
        }
    }

    public class SelectTile : ICommand
    {
        public TileUnionImpl Tile;
        private SelectedTileWrapper selectedTileCover;

        public SelectTile(Ray ray, SelectedTileWrapper selectedTileCover)
        {
            RaycastHit[] hits = Physics.RaycastAll(ray, float.PositiveInfinity);
            IEnumerable<TileUnionImpl> tiles = hits.ToList()
                .Where(x => x.collider.GetComponentInParent<TileUnionImpl>() != null)
                .Select(x => x.collider.GetComponentInParent<TileUnionImpl>());
            Tile = tiles.Count() != 0 ? tiles.First() : null;
            this.selectedTileCover = selectedTileCover;
        }

        public Result Execute(TileBuilderImpl tile_builder)
        {
            return tile_builder.SelectTile(Tile, selectedTileCover);
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
