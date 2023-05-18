﻿using System;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "FixedRoomConfig", menuName = "Level/Room/FixedRoomConfig", order = 0)]
public class FixedRoomConfig : AbstractRoomConfig
{
    [SerializeField] private RoomShopUI roomShopUI;

    public override RoomConfig GetRoomConfig()
    {
        return new RoomConfig(roomShopUI);
    }
}

