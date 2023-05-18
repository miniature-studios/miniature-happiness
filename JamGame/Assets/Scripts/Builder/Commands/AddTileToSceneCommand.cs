using Common;
using UnityEngine;

public class AddTileToSceneCommand : ICommand
{
    public GameObject tilePrefab;
    public Vector2Int CreatingPosition;
    public int CreatingRotation;
    public Ray ray;
    public AddTileToSceneCommand(GameObject tilePrefab, Ray ray)
    {
        this.tilePrefab = tilePrefab;
        CreatingPosition = new();
        CreatingRotation = 0;
        this.ray = ray;
    }
    public AddTileToSceneCommand(GameObject tilePrefab)
    {
        this.tilePrefab = tilePrefab;
        CreatingPosition = new();
        CreatingRotation = 0;
    }
    public Result Execute(TileBuilderController tileBuilderController)
    {
        return tileBuilderController.TileBuilder.AddTileIntoBuilding(tilePrefab, CreatingPosition, CreatingRotation);
    }
}