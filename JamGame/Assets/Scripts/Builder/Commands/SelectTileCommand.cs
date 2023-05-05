using Common;
using System.Linq;
using UnityEngine;

public class SelectTileCommand : ICommand
{
    public TileUnion tile;
    public SelectTileCommand(Ray ray)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, float.PositiveInfinity);
        System.Collections.Generic.IEnumerable<TileUnion> tiles = hits.ToList()
            .Where(x => x.collider.GetComponentInParent<TileUnion>() != null)
            .Select(x => x.collider.GetComponentInParent<TileUnion>());
        tile = tiles.Count() != 0 ? tiles.First() : null;
    }
    public Result Execute(TileBuilder tileBuilder)
    {
        return tile == null ? new FailResult("No hits") : tileBuilder.SelectTile(tile);
    }
}