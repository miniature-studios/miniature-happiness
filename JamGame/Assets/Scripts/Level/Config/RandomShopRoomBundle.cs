using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Room;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Config
{
    [CreateAssetMenu(fileName = "RandomShopRoomBundle", menuName = "Level/Random Shop Room Bundle")]
    public class RandomShopRoomBundle : ScriptableObject
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
}
