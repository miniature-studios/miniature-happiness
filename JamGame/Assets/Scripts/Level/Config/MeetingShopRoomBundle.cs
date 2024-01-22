using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level.Config
{
    [CreateAssetMenu(
        fileName = "MeetingShopRoomBundle",
        menuName = "Level/Meeting Shop Room Bundle",
        order = 0
    )]
    public class MeetingShopRoomBundle : ScriptableObject
    {
        [SerializeReference]
        private List<IShopRoomConfig> shopRooms = new();

        public IEnumerable<ShopRoomConfig> GetShopRooms()
        {
            return shopRooms.Select(x => x.GetRoomConfig());
        }
    }
}
