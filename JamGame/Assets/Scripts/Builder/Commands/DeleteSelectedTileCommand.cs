using UnityEngine;

public class DeleteSelectedTileCommand : ICommand
{
    TileBuilder tileBuilder;
    GameObject destroyedTile; // TODO
    public DeleteSelectedTileCommand(TileBuilder tileBuilder, ref GameObject destroyedTile)
    {
        this.tileBuilder = tileBuilder;
        this.destroyedTile = destroyedTile;
    }
    public void Execute()
    {
        tileBuilder.DeleteSelectedTile();
    }
}
