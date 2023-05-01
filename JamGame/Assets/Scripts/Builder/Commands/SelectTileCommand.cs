using Common;
using System.Linq;
using UnityEngine;

public class SelectTileCommand : ICommand
{
    TileBuilder tileBuilder;
    float rayDistance = 400f;
    public Tile tile;
    public SelectTileCommand(TileBuilder tileBuilder, Ray ray)
    {
        this.tileBuilder = tileBuilder;
        var hits = Physics.RaycastAll(ray, rayDistance);
        var roomHits = hits.ToList().Where(x => x.collider.GetComponent<Tile>() != null);
        if (roomHits.Count() != 0)
        {
            var tile = hits.ToList().Find(x => x.collider.GetComponent<Tile>()).collider.GetComponent<Tile>();
            this.tile = tile;
        }
        else
        {
            tile = null;
        }
    }
    public Response Execute()
    {
        if(tile == null)
            return new Response("No hits", false);
        return tileBuilder.SelectTile(tile);
    }
}