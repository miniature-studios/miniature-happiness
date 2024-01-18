using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Room;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Config
{
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
        [SerializeField]
        private float weight;
        public float Weight => weight;

        [AssetSelector]
        [AssetsOnly]
        [SerializeField]
        private CoreModel room;
        public CoreModel Room => room;
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
