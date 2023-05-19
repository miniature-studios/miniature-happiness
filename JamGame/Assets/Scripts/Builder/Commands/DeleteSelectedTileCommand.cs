using Common;
using System;

public class DeleteSelectedTileCommand : ICommand
{
    private TileUI tileUIPrefab;
    private readonly Action<TileUI> sendUIPrefab;
    public DeleteSelectedTileCommand(Action<TileUI> send_ui_prefab)
    {
        sendUIPrefab = send_ui_prefab;
    }
    public Result Execute(TileBuilderController tile_builder_controller)
    {
        Result response = tile_builder_controller.TileBuilder.DeleteSelectedTile(out tileUIPrefab);
        sendUIPrefab(tileUIPrefab);
        return response;
    }
}
