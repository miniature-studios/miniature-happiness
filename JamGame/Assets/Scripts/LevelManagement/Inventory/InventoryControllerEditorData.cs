using System;
using System.Collections.Generic;

[Serializable]
public class NamedRoomInventoriUI
{
    public string Name;
    public RoomInventoryUI RIUI;
}

public partial class InventoryController
{
    public List<NamedRoomInventoriUI> NamedRoomInventoryUIs;

    public void CreateGodsInventory()
    {
        foreach (NamedRoomInventoriUI item in NamedRoomInventoryUIs)
        {
            for (int i = 0; i < 666; i++)
            {
                CreateOneUI(item.RIUI);
            }
        }
    }

    public void CreateOneUI(RoomInventoryUI RIUI)
    {
        AddNewRoom(RIUI);
    }
}
