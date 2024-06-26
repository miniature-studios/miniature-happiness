using System;
using Level.Room;
using UnityEngine;

namespace Level.Config
{
    [Serializable]
    public class InventoryRoomConfig
    {
        [SerializeField]
        private CoreModel room;

        public CoreModel Room => room;
    }
}
