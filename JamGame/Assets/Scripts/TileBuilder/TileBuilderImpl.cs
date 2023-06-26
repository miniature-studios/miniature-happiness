using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TileUnion;
using UnityEngine;

namespace TileBuilder
{
    public partial class TileBuilderImpl : MonoBehaviour
    {
        [SerializeField]
        private TileUnionImpl freespacePrefab;

        [SerializeField]
        private GameObject rootObject;

        [SerializeField]
        private BuilderMatrix builderMatrix;

        private List<Vector2Int> previousPlaces = new();
        private int previousRotation = 0;
        private bool justCreated = false;

        public TileUnionImpl SelectedTile { get; private set; } = null;
        public GameObject RootObject
        {
            get => rootObject;
            set => rootObject = value;
        }
        public BuilderMatrix BuilderMatrix => builderMatrix;
        public Dictionary<Vector2Int, TileUnionImpl> TileUnionDictionary { get; } = new();

        public void Start()
        {
            foreach (TileUnionImpl union in rootObject.GetComponentsInChildren<TileUnionImpl>())
            {
                if (!TileUnionDictionary.Values.Contains(union))
                {
                    foreach (Vector2Int pos in union.TilesPositions)
                    {
                        TileUnionDictionary.Add(pos, union);
                    }
                }
            }

            InspectorStart();
            UpdateAllTiles();
        }

        public Result Validate()
        {
            if (SelectedTile != null)
            {
                return new FailResult("Complete placing first");
            }

            Stack<KeyValuePair<Vector2Int, TileUnionImpl>> points_stack =
                new(TileUnionDictionary.Where(x => x.Value.IsAllWithMark("door")));
            List<KeyValuePair<Vector2Int, TileUnionImpl>> tiles_to_check = TileUnionDictionary
                .Where(
                    x => !x.Value.IsAllWithMark("Outside") && !x.Value.IsAllWithMark("freespace")
                )
                .ToList();

            while (points_stack.Count > 0)
            {
                KeyValuePair<Vector2Int, TileUnionImpl> point = points_stack.Pop();
                foreach (
                    Direction dir in point.Value.GetAccessibleDirectionsFromPosition(point.Key)
                )
                {
                    List<KeyValuePair<Vector2Int, TileUnionImpl>> near_tiles =
                        new(tiles_to_check.Where(x => x.Key == dir.ToVector2Int() + point.Key));
                    if (near_tiles.Count() > 0)
                    {
                        foreach (KeyValuePair<Vector2Int, TileUnionImpl> founded_tile in near_tiles)
                        {
                            _ = tiles_to_check.Remove(founded_tile);
                            points_stack.Push(founded_tile);
                        }
                    }
                }
            }

            if (tiles_to_check.Count > 0)
            {
                foreach (TileUnionImpl union in tiles_to_check.Select(x => x.Value).Distinct())
                {
                    union.ShowInvalidPlacing();
                }
                return new FailResult("Some tiles not connected");
            }
            else
            {
                return new SuccessResult();
            }
        }

        public Result SelectTile(TileUnionImpl tile)
        {
            if (SelectedTile == tile)
            {
                return new FailResult("Selected already selected tile");
            }
            else if (tile == null)
            {
                return new FailResult("Null");
            }
            else
            {
                if (SelectedTile != null)
                {
                    Result result = ComletePlacing();
                    if (result.Failure)
                    {
                        return result;
                    }
                }
                SelectedTile = tile;
                SelectedTile.ApplySelecting();
                SelectedTile.IsolateUpdate();
                previousPlaces = SelectedTile.TilesPositions.ToList();
                previousRotation = SelectedTile.Rotation;
                return new SuccessResult();
            }
        }

        public Result DeleteSelectedTile(out RoomInventoryUI deleted_tile_ui)
        {
            if (SelectedTile == null)
            {
                deleted_tile_ui = null;
                return new FailResult("Not selected Tile");
            }
            if (justCreated)
            {
                justCreated = false;
                deleted_tile_ui = DeleteTile(SelectedTile);
                SelectedTile = null;
                return new SuccessResult();
            }
            else
            {
                justCreated = false;
                deleted_tile_ui = DeleteTile(SelectedTile);
                foreach (Vector2Int pos in previousPlaces)
                {
                    _ = TileUnionDictionary.Remove(pos);
                }
                foreach (Vector2Int position in previousPlaces)
                {
                    CreateTileAndBind(freespacePrefab, position, 0);
                }
                UpdateSidesInPositions(previousPlaces);
                SelectedTile = null;
                return new SuccessResult();
            }
        }

        public Result MoveSelectedTile(Direction direction)
        {
            if (SelectedTile == null)
            {
                return new FailResult("Not selected Tile");
            }
            else
            {
                SelectedTile.Move(direction);
                SelectedTile.ApplySelecting();
                _ = SelectedTile.TryApplyErrorTiles(this);
                return new SuccessResult();
            }
        }

        public Result RotateSelectedTile(RotationDirection direction)
        {
            if (SelectedTile == null)
            {
                return new FailResult("Not selected Tile");
            }
            else
            {
                SelectedTile.SetRotation(SelectedTile.Rotation + (int)direction);
                SelectedTile.ApplySelecting();
                _ = SelectedTile.TryApplyErrorTiles(this);
                return new SuccessResult();
            }
        }

        public Result ComletePlacing()
        {
            if (SelectedTile == null)
            {
                return new FailResult("Not selected Tile");
            }
            if (
                previousPlaces.Intersect(SelectedTile.TilesPositions).Count()
                    == previousPlaces.Count
                && previousRotation == SelectedTile.Rotation
                && !justCreated
            )
            {
                UpdateSidesInPositions(SelectedTile.TilesPositionsForUpdating);
                SelectedTile.CancelSelecting();
                SelectedTile = null;
                return new SuccessResult();
            }
            List<TileUnionImpl> tilesUnder = TileUnionDictionary
                .Where(x => SelectedTile.TilesPositions.Contains(x.Key))
                .Select(x => x.Value)
                .Distinct()
                .ToList();
            _ = tilesUnder.Remove(SelectedTile);
            if (!tilesUnder.All(x => x.IsAllWithMark("freespace")))
            {
                return new FailResult("Not free spaces under");
            }
            if (SelectedTile.TryApplyErrorTiles(this).Success)
            {
                return new FailResult("Cannot place tiles");
            }
            while (tilesUnder.Count > 0)
            {
                TileUnionImpl buffer = tilesUnder.Last();
                _ = tilesUnder.Remove(buffer);
                _ = DeleteTile(buffer);
            }
            if (!justCreated)
            {
                List<Vector2Int> bufferPositions = new();
                foreach (Vector2Int position in previousPlaces)
                {
                    if (TileUnionDictionary.ContainsKey(position))
                    {
                        _ = TileUnionDictionary.Remove(position);
                        if (!SelectedTile.TilesPositions.Contains(position))
                        {
                            bufferPositions.Add(position);
                        }
                    }
                }
                foreach (Vector2Int pos in bufferPositions)
                {
                    CreateTileAndBind(freespacePrefab, pos, 0);
                }
            }
            foreach (Vector2Int pos in SelectedTile.TilesPositions)
            {
                if (TileUnionDictionary.TryGetValue(pos, out TileUnionImpl tileUnion))
                {
                    RemoveTileFromDictionary(tileUnion);
                }
                TileUnionDictionary.Add(pos, SelectedTile);
            }
            UpdateSidesInPositions(SelectedTile.TilesPositionsForUpdating);
            if (!justCreated)
            {
                UpdateSidesInPositions(previousPlaces);
            }
            SelectedTile.CancelSelecting();
            SelectedTile = null;
            justCreated = false;
            return new SuccessResult();
        }

        public Result AddTileIntoBuilding(
            TileUnionImpl tile_prefab,
            Vector2Int position,
            int rotation
        )
        {
            if (SelectedTile != null)
            {
                return new FailResult("Complete placing previous tile");
            }
            justCreated = true;
            TileUnionImpl tile = CreateTile(tile_prefab, position, rotation);
            return SelectTile(tile);
        }

        public void UpdateAllTiles()
        {
            foreach (KeyValuePair<Vector2Int, TileUnionImpl> pair in TileUnionDictionary)
            {
                pair.Value.UpdateWalls(this, pair.Key);
            }
            foreach (KeyValuePair<Vector2Int, TileUnionImpl> pair in TileUnionDictionary)
            {
                pair.Value.UpdateCorners(this, pair.Key);
            }
        }

        public IEnumerable<TileUnionImpl> GetTileUnionsInPositions(
            IEnumerable<Vector2Int> positions
        )
        {
            return TileUnionDictionary
                .Where(x => positions.Contains(x.Key))
                .Select(x => x.Value)
                .Distinct();
        }

        public IEnumerable<Vector2Int> GetFreeSpaceInsideListPositions()
        {
            return TileUnionDictionary
                .Where(x => x.Value.IsAllWithMark("freespace"))
                .Select(x => x.Key)
                .OrderBy(x => Vector2Int.Distance(x, new(0, 0)));
        }

        public IEnumerable<Vector2Int> GetAllInsideListPositions()
        {
            return TileUnionDictionary
                .Where(x => !x.Value.IsAllWithMark("Outside"))
                .Select(x => x.Key);
        }

        public void CreateTileAndBind(TileUnionImpl tile_prefab, Vector2Int position, int rotation)
        {
            TileUnionImpl tileUnion = CreateTile(tile_prefab, position, rotation);
            foreach (Vector2Int pos in tileUnion.TilesPositions)
            {
                TileUnionDictionary.Add(pos, tileUnion);
            }
            UpdateSidesInPositions(tileUnion.TilesPositionsForUpdating);
        }

        private TileUnionImpl CreateTile(
            TileUnionImpl tile_prefab,
            Vector2Int position,
            int rotation
        )
        {
            TileUnionImpl tileUnion = Instantiate(tile_prefab, rootObject.transform);
            tileUnion.SetPosition(position);
            tileUnion.SetRotation(rotation);
            return tileUnion;
        }

        private RoomInventoryUI DeleteTile(TileUnionImpl tile_union)
        {
            if (tile_union == null)
            {
                return null;
            }
            RoomInventoryUI UIPrefab = tile_union.UIPrefab;
            DestroyImmediate(tile_union.gameObject);
            RemoveTileFromDictionary(tile_union);
            return UIPrefab;
        }

        private void RemoveTileFromDictionary(TileUnionImpl tile_union)
        {
            foreach (KeyValuePair<Vector2Int, TileUnionImpl> item in TileUnionDictionary)
            {
                if (item.Value == tile_union)
                {
                    _ = TileUnionDictionary.Remove(item.Key);
                    break;
                }
            }
        }

        private void UpdateSidesInPositions(IEnumerable<Vector2Int> positions)
        {
            List<(TileUnionImpl, Vector2Int)> queue = new();
            foreach (Vector2Int position in positions)
            {
                if (TileUnionDictionary.TryGetValue(position, out TileUnionImpl tile))
                {
                    tile.UpdateWalls(this, position);
                    queue.Add((tile, position));
                }
            }
            foreach ((TileUnionImpl, Vector2Int) pair in queue)
            {
                pair.Item1.UpdateCorners(this, pair.Item2);
            }
        }
    }
}
