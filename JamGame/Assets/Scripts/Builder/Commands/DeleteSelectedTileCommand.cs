using Common;
using System;

public class DeleteSelectedTileCommand : ICommand
{
    private TileUI tileUIPrefab;
    private readonly Action<TileUI> sendUIPrefab;
    public DeleteSelectedTileCommand(Action<TileUI> sendUIPrefab)
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
