using Common;
using UnityEngine;

public class AddTileToSceneCommand : ICommand
{
    public TileUnion TilePrefab;
    public Vector2Int CreatingPosition;
    public int CreatingRotation;
    public Ray Ray;
    public AddTileToSceneCommand(TileUnion tile_prefab, Ray ray)
    {
        TilePrefab = tile_prefab;
        CreatingPosition = new();
        CreatingRotation = 0;
        Ray = ray;
    }
    public AddTileToSceneCommand(TileUnion tile_prefab)
    {
        TilePrefab = tile_prefab;
        CreatingPosition = new();
        CreatingRotation = 0;
    }
    public Result Execute(TileBuilderController tile_builder_controller)
    {
        return tile_builder_controller.TileBuilder.AddTileIntoBuilding(TilePrefab, CreatingPosition, CreatingRotation);
    }
}