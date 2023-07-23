using System;
using UnityEngine;

namespace Level.Config 
{
    [Serializable]
    public class SerializedShopRoomConfig
    {
        [SerializeField] 
        private string selectedType;

        [SerializeField]
        private FixedRoomConfig fixedRoomConfig;

        [SerializeField]
        private RandomRoomConfig randomRoomConfig;

        public IShopRoomConfig ToShopRoomConfig()
        {
            return selectedType switch
            {
                "FixedRoomConfig" => fixedRoomConfig,
                "RandomRoomConfig" => randomRoomConfig,
                _ => throw new NotImplementedException(),
            };
        }
    }
}
