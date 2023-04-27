using UnityEngine;

public class DeleteSelectedTileCommand : ICommand
{
    TileBuilder tileBuilder;
    GameObject destroyedTile;
    public DeleteSelectedTileCommand(TileBuilder tileBuilder, ref GameObject destroyedTile)
    {
        this.tileBuilder = tileBuilder;
        this.destroyedTile = destroyedTile;
    }
    public Answer Execute()
    {
        return tileBuilder.DeleteSelectedTile(out destroyedTile);
    }
}
