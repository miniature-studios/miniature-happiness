using Level.Room;
using System;

namespace Level.Config
{
    [Serializable]
    public class InventoryRoomConfig
    {
        public CoreModel Room { get; }

        public InventoryRoomConfig(CoreModel model)
        {
            Room = model;
        }
    }
}
