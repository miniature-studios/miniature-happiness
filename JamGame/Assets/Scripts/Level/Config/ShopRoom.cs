using Common;
using Level.Room;
using Pickle;
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
        [Pickle(LookupType = ObjectProviderType.Assets)]
        private CoreModel room;

        public ShopRoomConfig GetRoomConfig()
        {
            return new ShopRoomConfig(room);
        }
    }

    [Serializable]
    public class RoomWeights
    {
        public float Weight;

        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel Room;
    }

    [Serializable]
    public class RandomRoomConfig : IShopRoomConfig
    {
        [SerializeField]
        private List<RoomWeights> roomWeights;

        public ShopRoomConfig GetRoomConfig()
        {
            List<float> list = roomWeights.Select(x => x.Weight).ToList();
            CoreModel result = roomWeights[RandomTools.RandomlyChooseWithWeights(list)].Room;
            return new ShopRoomConfig(result);
        }
    }

    public class ShopRoomConfig
    {
        public CoreModel Room { get; }

        public ShopRoomConfig(CoreModel model)
        {
            Room = model;
        }
    }
}
