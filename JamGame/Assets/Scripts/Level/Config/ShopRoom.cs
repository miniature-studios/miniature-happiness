using Common;
using Level.Room;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level.Config
{
    [InterfaceEditor]
    [HideReferenceObjectPicker]
    public interface IShopRoomConfig
    {
        public ShopRoomConfig GetRoomConfig();
    }

    [Serializable]
    public class FixedRoomConfig : IShopRoomConfig
    {
        [SerializeField]
        [AssetSelector]
        [AssetsOnly]
        [FoldoutGroup("@Label")]
        private CoreModel room;

        private string Label => $"Fixed Room - {room?.Title}";

        public ShopRoomConfig GetRoomConfig()
        {
            return new ShopRoomConfig(room);
        }
    }

    [Serializable]
    [HideReferenceObjectPicker]
    public class RoomWeights
    {
        [OdinSerialize]
        public float Weight { get; private set; }

        [AssetSelector]
        [AssetsOnly]
        [OdinSerialize]
        public CoreModel Room { get; private set; }
    }

    [Serializable]
    public class RandomRoomConfig : IShopRoomConfig
    {
        [SerializeField]
        [FoldoutGroup("Random Room")]
        private List<RoomWeights> roomWeights = new();

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
