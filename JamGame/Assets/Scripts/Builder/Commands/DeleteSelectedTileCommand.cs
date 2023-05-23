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
    public Result Execute(TileBuilderController tile_builder_controller)
    {
        Result response = tile_builder_controller.TileBuilder.DeleteSelectedTile(out tileUIPrefab);
        sendUIPrefab(tileUIPrefab);
        return response;
    }
}
