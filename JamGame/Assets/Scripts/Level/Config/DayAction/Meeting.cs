using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level.Config.DayAction
{
    [Serializable]
    public class Meeting : IDayAction
    {
        [SerializeReference]
        private List<IEmployeeConfig> shopEmployees = new();
        public IEnumerable<EmployeeConfig> ShopEmployees =>
            shopEmployees.Select(x => x.GetEmployeeConfig());

        [SerializeReference]
        private List<IShopRoomConfig> shopRooms = new();
        public IEnumerable<ShopRoomConfig> ShopRooms => shopRooms.Select(x => x.GetRoomConfig());

        [SerializeField]
        private List<InventoryRoomConfig> mandatoryRooms = new();
        public IEnumerable<InventoryRoomConfig> MandatoryRooms => mandatoryRooms;

        public void Execute(Executor executor)
        {
            executor.Execute(this);
        }
    }
}
