using System;

namespace Level.Config
{
    [Serializable]
    public class InventoryRoomConfig
    {
        public Inventory.Room.Model Room { get; }

        public InventoryRoomConfig(Inventory.Room.Model model)
        {
            Room = model;
        }
    }
}