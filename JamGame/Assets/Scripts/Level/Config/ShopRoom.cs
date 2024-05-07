using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Room;
using Pickle;
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
        [AssetsOnly]
        [SerializeField]
        [Pickle(typeof(CoreModel), LookupType = ObjectProviderType.Assets)]
        [FoldoutGroup("@" + nameof(Label))]
        private CoreModel room;
        private string Label => $"Fixed Room - {(room == null ? "NULL" : room.RoomInfo.Title)}";

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

        [AssetsOnly]
        [SerializeField]
        [Pickle(typeof(CoreModel), LookupType = ObjectProviderType.Assets)]
        private CoreModel room;
        public CoreModel Room => room;
    }

    [Serializable]
    public class RandomRoomConfig : IShopRoomConfig
    {
        private enum SourceMode
        {
            Raw,
            ScriptableObject
        }

        [EnumToggleButtons]
        [SerializeField]
        [FoldoutGroup("Random Room")]
        private SourceMode mode = SourceMode.Raw;

        [SerializeField]
        [FoldoutGroup("Random Room")]
        [ShowIf("@" + nameof(mode), SourceMode.Raw)]
        private List<RoomWeights> roomWeights = new();

        [SerializeField]
        [FoldoutGroup("Random Room")]
        [ShowIf("@" + nameof(mode), SourceMode.ScriptableObject)]
        private RandomShopRoomBundle randomShopRoomBundle;

        public ShopRoomConfig GetRoomConfig()
        {
            if (mode == SourceMode.ScriptableObject)
            {
                return randomShopRoomBundle.GetRoomConfig();
            }
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
