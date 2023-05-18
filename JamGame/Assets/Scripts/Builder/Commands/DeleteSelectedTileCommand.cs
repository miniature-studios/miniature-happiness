using Common;
using System;
using UnityEngine;

public class DeleteSelectedTileCommand : ICommand
{
    private GameObject tileUIPrefab;
    private readonly Action<GameObject> sendUIPrefab;
    public DeleteSelectedTileCommand(Action<GameObject> sendUIPrefab)
    {
        this.sendUIPrefab = sendUIPrefab;
    }
    public Result Execute(TileBuilderController tileBuilderController)
    {
        Result response = tileBuilderController.TileBuilder.DeleteSelectedTile(out tileUIPrefab);
        sendUIPrefab(tileUIPrefab);
        return response;
    }
}
