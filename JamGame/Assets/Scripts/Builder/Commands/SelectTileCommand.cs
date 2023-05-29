using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectTileCommand : ICommand
{
    public TileUnion Tile;
    public SelectTileCommand(Ray ray)
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