using Common;
using System.Linq;
using UnityEngine;

public class SelectTileCommand : ICommand
{
    TileBuilder tileBuilder;
    float rayDistance = 300;
    public TileUnion tile;
    public SelectTileCommand(TileBuilder tileBuilder, Ray ray)
    {
        this.tileBuilder = tileBuilder;
        var hits = Physics.RaycastAll(ray, rayDistance);
        var tiles = hits.ToList()
            .Where(x => x.collider.GetComponentInParent<TileUnion>() != null)
            .Select(x => x.collider.GetComponentInParent<TileUnion>());
        if (tiles.Count() != 0)
        {
            this.tile = tiles.First();
        }
        else
        {
            tile = null;
        }
    }
    public Result Execute()
    {
        if(tile == null)
            return new FailResult("No hits");
        return tileBuilder.SelectTile(tile);
    }
}