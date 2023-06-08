using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class RoomWeights
{
    public float Weight;
    public RoomShopUI RoomShopUI;
}

[Serializable]
public class RandomRoomConfig : IRoomConfig
{
    [SerializeField]
    private List<RoomWeights> roomWeights;

    public RoomConfig GetRoomConfig()
    {
        List<float> list = roomWeights.Select(x => x.Weight).ToList();
        RoomShopUI result = roomWeights[RandomTools.RandomlyChooseWithWeights(list)].RoomShopUI;
        return new RoomConfig(result);
    }
}
