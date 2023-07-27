using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using TileUnion;
using UnityEngine;

namespace TileBuilder
{
    [AddComponentMenu("TileBuilder.TileBuilder")]
    public partial class TileBuilderImpl : MonoBehaviour
    {
        // Public for inspector`
        public TileUnionImpl FreespacePrefab;

        [SerializeField]
        private GameObject rootObject;

        [SerializeField]
        private Matrix builderMatrix;

        private List<Vector2Int> previousPlaces = new();
        private int previousRotation = 0;
        private bool justCreated = false;

        public GameObject RootObject
        {
            get => rootObject;
            set => rootObject = value;
        }
        public Matrix BuilderMatrix => builderMatrix;
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
            UpdateAllTiles();
        }

        public Result Validate()
        {
            Stack<KeyValuePair<Vector2Int, TileUnionImpl>> points_stack =
                new(TileUnionDictionary.Where(x => x.Value.IsAllWithMark("door")));
            List<KeyValuePair<Vector2Int, TileUnionImpl>> tiles_to_check = TileUnionDictionary
                .Where(
                    x => !x.Value.IsAllWithMark("Outside") && !x.Value.IsAllWithMark("Freespace")
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

        public Result SelectTile(TileUnionImpl tile, ref TileUnionImpl selectedTile)
        {
            selectedTile = tile;
            selectedTile.ApplySelecting();
            selectedTile.IsolateUpdate();
            previousPlaces = selectedTile.TilesPositions.ToList();
            previousRotation = selectedTile.Rotation;
            return new SuccessResult();
        }

        public Result DeleteSelectedTile(
            out Level.Inventory.Room.Model deleted_tile,
            ref TileUnionImpl selected_tile
        )
        {
            if (justCreated)
            {
                justCreated = false;
                _ = DeleteTile(ref selected_tile);
                selected_tile = null;
                deleted_tile = null;
                return new SuccessResult();
            }
            else
            {
                justCreated = false;
                deleted_tile = DeleteTile(ref selected_tile);
                foreach (Vector2Int pos in previousPlaces)
                {
                    _ = TileUnionDictionary.Remove(pos);
                }
                foreach (Vector2Int position in previousPlaces)
                {
                    CreateTileAndBind(FreespacePrefab, position, 0);
                }
                UpdateSidesInPositions(previousPlaces);
                selected_tile = null;
                deleted_tile = null;
                return new SuccessResult();
            }
        }

        public Result MoveSelectedTile(Direction direction, ref TileUnionImpl selected_tile)
        {
            selected_tile.Move(direction);
            selected_tile.ApplySelecting();
            _ = selected_tile.TryApplyErrorTiles(this);
            return new SuccessResult();
        }

        public Result RotateSelectedTile(
            RotationDirection direction,
            ref TileUnionImpl selected_tile
        )
        {
            selected_tile.SetRotation(selected_tile.Rotation + (int)direction);
            selected_tile.ApplySelecting();
            _ = selected_tile.TryApplyErrorTiles(this);
            return new SuccessResult();
        }

        public Result ComletePlacing(ref TileUnionImpl selected_tile)
        {
            if (
                previousPlaces.Intersect(selected_tile.TilesPositions).Count()
                    == previousPlaces.Count
                && previousRotation == selected_tile.Rotation
                && !justCreated
            )
            {
                UpdateSidesInPositions(selected_tile.TilesPositionsForUpdating);
                selected_tile.CancelSelecting();
                selected_tile = null;
                return new SuccessResult();
            }
            IEnumerable<Vector2Int> vector2Ints = selected_tile.TilesPositions;
            List<TileUnionImpl> tilesUnder = TileUnionDictionary
                .Where(x => vector2Ints.Contains(x.Key))
                .Select(x => x.Value)
                .Distinct()
                .ToList();
            _ = tilesUnder.Remove(selected_tile);
            if (!tilesUnder.All(x => x.IsAllWithMark("Freespace")))
            {
                return new FailResult("Not free spaces under");
            }
            if (selected_tile.TryApplyErrorTiles(this).Success)
            {
                return new FailResult("Cannot place tiles");
            }
            while (tilesUnder.Count > 0)
            {
                TileUnionImpl buffer = tilesUnder.Last();
                _ = tilesUnder.Remove(buffer);
                _ = DeleteTile(ref buffer);
            }
            if (!justCreated)
            {
                List<Vector2Int> bufferPositions = new();
                foreach (Vector2Int position in previousPlaces)
                {
                    if (TileUnionDictionary.ContainsKey(position))
                    {
                        _ = TileUnionDictionary.Remove(position);
                        if (!selected_tile.TilesPositions.Contains(position))
                        {
                            bufferPositions.Add(position);
                        }
                    }
                }
                foreach (Vector2Int pos in bufferPositions)
                {
                    CreateTileAndBind(FreespacePrefab, pos, 0);
                }
            }
            foreach (Vector2Int pos in selected_tile.TilesPositions)
            {
                if (TileUnionDictionary.TryGetValue(pos, out TileUnionImpl tileUnion))
                {
                    RemoveTileFromDictionary(tileUnion);
                }
                TileUnionDictionary.Add(pos, selected_tile);
            }
            UpdateSidesInPositions(selected_tile.TilesPositionsForUpdating);
            if (!justCreated)
            {
                UpdateSidesInPositions(previousPlaces);
            }
            selected_tile.CancelSelecting();
            selected_tile = null;
            justCreated = false;
            return new SuccessResult();
        }

        public Result AddTileIntoBuilding(
            TileUnionImpl tile_prefab,
            ref TileUnionImpl selectedTile,
            Vector2Int position,
            int rotation
        )
        {
            justCreated = true;
            TileUnionImpl tile = CreateTile(tile_prefab, position, rotation);
            return SelectTile(tile, ref selectedTile);
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
                .Where(x => x.Value.IsAllWithMark("Freespace"))
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

        // Public for inspector
        public Level.Inventory.Room.Model DeleteTile(ref TileUnionImpl tile_union)
        {
            if (tile_union == null)
            {
                return null;
            }
            Level.Inventory.Room.Model UIPrefab = tile_union.InventoryModel;
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
