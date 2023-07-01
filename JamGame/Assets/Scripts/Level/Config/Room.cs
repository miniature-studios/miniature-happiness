using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level.Config
{
    [InterfaceEditor]
    public interface IRoomConfig
    {
        public RoomConfig GetRoomConfig();
    }

    [Serializable]
    public class FixedRoomConfig : IRoomConfig
    {
        [SerializeField]
        private Shop.Room.View roomShopUI;

        public RoomConfig GetRoomConfig()
        {
            return new RoomConfig(roomShopUI);
        }
    }

    [Serializable]
    public class RoomWeights
    {
        public float Weight;
        public Shop.Room.View RoomShopUI;
    }

    [Serializable]
    public class RandomRoomConfig : IRoomConfig
    {
        [SerializeField]
        private List<RoomWeights> roomWeights;

        public RoomConfig GetRoomConfig()
        {
            List<float> list = roomWeights.Select(x => x.Weight).ToList();
            Shop.Room.View result = roomWeights[
                RandomTools.RandomlyChooseWithWeights(list)
            ].RoomShopUI;
            return new RoomConfig(result);
        }
    }

    public class RoomConfig
    {
        public Shop.Room.View RoomShopUI { get; }

        public RoomConfig(Shop.Room.View room_shop_ui)
        {
            RoomShopUI = room_shop_ui;
        }
    }
}
