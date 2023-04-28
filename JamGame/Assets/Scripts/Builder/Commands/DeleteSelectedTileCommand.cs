using UnityEngine;
using Common;
using System;

public class DeleteSelectedTileCommand : ICommand
{
    TileBuilder tileBuilder;
    GameObject destroyedTile;
    Action<GameObject> sendDestroyedTile;
    public DeleteSelectedTileCommand(TileBuilder tileBuilder, Action<GameObject> sendDestroyedTile)
    {
        this.tileBuilder = tileBuilder;
        this.sendDestroyedTile = sendDestroyedTile;
    }
    public Response Execute()
    {
        var response = tileBuilder.DeleteSelectedTile(out destroyedTile);
        sendDestroyedTile(destroyedTile);
        return response;
    }
}
