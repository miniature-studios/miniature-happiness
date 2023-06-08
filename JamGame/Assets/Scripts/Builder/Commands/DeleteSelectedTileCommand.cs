using Common;
using System;

public class DeleteSelectedTileCommand : ICommand
{
    private RoomInventoryUI tileUIPrefab;
    private readonly Action<RoomInventoryUI> sendUIPrefab;

    public DeleteSelectedTileCommand(Action<RoomInventoryUI> send_ui_prefab)
    {
        sendUIPrefab = send_ui_prefab;
    }

    public Result Execute(TileBuilder tile_builder)
    {
        Result response = tile_builder.DeleteSelectedTile(out tileUIPrefab);
        sendUIPrefab(tileUIPrefab);
        return response;
    }
}
