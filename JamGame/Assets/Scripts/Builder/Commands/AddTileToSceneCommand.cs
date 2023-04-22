using System.Linq;
using UnityEngine;

public class AddTileToSceneCommand : ICommand
{
    TileBuilder tileBuilder;
    GameObject tilePrefab;
    public AddTileToSceneCommand(TileBuilder tileBuilder, GameObject tilePrefab)
    {
        this.tileBuilder = tileBuilder;
        this.tilePrefab = tilePrefab;
    }
    public void SetParameters(GameObject tilePrefab)
    {
        this.tilePrefab = tilePrefab;
    }
    public void Execute()
    {
        tileBuilder.CreateTile(tilePrefab, tileBuilder.GetInsideListPositions().First(), 0);
    }
}