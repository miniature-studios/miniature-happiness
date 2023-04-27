using System.Linq;
using UnityEngine;

public class AddTileToSceneCommand : ICommand
{
    TileBuilder tileBuilder;
    public GameObject TilePrefab;
    public Vector2Int CreatingPosition;
    public int CreatingRotation;
    public AddTileToSceneCommand(TileBuilder tileBuilder, GameObject tilePrefab)
    {
        this.tileBuilder = tileBuilder;
        this.TilePrefab = tilePrefab;
        CreatingPosition = new();
        CreatingRotation = 0;
    }
    public Answer Execute()
    {
        return tileBuilder.AddTileIntoBuilding(TilePrefab, CreatingPosition, CreatingRotation);
    }
}