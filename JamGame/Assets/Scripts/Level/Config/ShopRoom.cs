using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level.Config
{
    [InterfaceEditor]
    public interface IShopRoomConfig
    {
        public ShopRoomConfig GetRoomConfig();
    }

    [Serializable]
    public class FixedRoomConfig : IShopRoomConfig
    {
        [SerializeField]
        private readonly Shop.Room.Model room;

        public ShopRoomConfig GetRoomConfig()
        {
            return new ShopRoomConfig(room);
        }
    }

    [Serializable]
    public class RoomWeights
    {
        public float Weight;
        public Shop.Room.Model Room;
    }

    [Serializable]
    public class RandomRoomConfig : IShopRoomConfig
    {
        [SerializeField]
        private readonly List<RoomWeights> roomWeights;

        public ShopRoomConfig GetRoomConfig()
        {
            List<float> list = roomWeights.Select(x => x.Weight).ToList();
            Shop.Room.Model result = roomWeights[
                RandomTools.RandomlyChooseWithWeights(list)
            ].Room;
            return new ShopRoomConfig(result);
        }
    }

    public class ShopRoomConfig
    {
        public Shop.Room.Model Room { get; }

        public ShopRoomConfig(Shop.Room.Model model)
        {
            Room = model;
        }
    }
}
