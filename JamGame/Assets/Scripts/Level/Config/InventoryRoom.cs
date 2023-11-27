using Level.Room;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;

namespace Level.Config
{
    [Serializable]
    [HideReferenceObjectPicker]
    public class InventoryRoomConfig
    {
        [AssetSelector]
        [OdinSerialize]
        public CoreModel Room { get; private set; }
    }
}
