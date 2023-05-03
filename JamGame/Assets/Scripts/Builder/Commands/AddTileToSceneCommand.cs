using UnityEngine;
using Common;

public class AddTileToSceneCommand : ICommand
{
    TileBuilder tileBuilder;
    public GameObject tilePrefab;
    public Vector2Int CreatingPosition;
    public int CreatingRotation;
    public Ray ray;
    public AddTileToSceneCommand(TileBuilder tileBuilder, GameObject tilePrefab, Ray ray)
    {
        this.tileBuilder = tileBuilder;
        this.tilePrefab = tilePrefab;
        CreatingPosition = new();
        CreatingRotation = 0;
        this.ray = ray;
    }
    public AddTileToSceneCommand(TileBuilder tileBuilder, GameObject tilePrefab)
    {
        this.tileBuilder = tileBuilder;
        this.tilePrefab = tilePrefab;
        CreatingPosition = new();
        CreatingRotation = 0;
    }
    public Result Execute()
    {
        return tileBuilder.AddTileIntoBuilding(tilePrefab, CreatingPosition, CreatingRotation);
    }
}