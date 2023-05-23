using System;
using UnityEngine;

[Serializable]
public class FixedRoomConfig : IRoomConfig
{
    [SerializeField] private RoomShopUI roomShopUI;

    public RoomConfig GetRoomConfig()
    {
        return new RoomConfig(roomShopUI);
    }
}

