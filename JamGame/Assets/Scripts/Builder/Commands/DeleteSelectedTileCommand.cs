using UnityEngine;
using Common;
using System;

public class DeleteSelectedTileCommand : ICommand
{
    TileBuilder tileBuilder;
    GameObject tileUIPrefab;
    Action<GameObject> sendUIPrefab;
    public DeleteSelectedTileCommand(TileBuilder tileBuilder, Action<GameObject> sendUIPrefab)
    {
        this.tileBuilder = tileBuilder;
        this.sendUIPrefab = sendUIPrefab;
    }
    public Result Execute()
    {
        var response = tileBuilder.DeleteSelectedTile(out tileUIPrefab);
        sendUIPrefab(tileUIPrefab);
        return response;
    }
}
