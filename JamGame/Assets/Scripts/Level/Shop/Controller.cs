using Common;
using Level.Config;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level.Shop
{
    [AddComponentMenu("Level.Shop.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private readonly Inventory.Controller inventoryController;

        [SerializeField]
        private readonly Model shopModel;

        [SerializeField]
        private readonly Finances.Model financesController;

        public void SetShopRooms(IEnumerable<ShopRoomConfig> room_configs)
        {
            shopModel.SetRooms(room_configs.Select(x => x.Room).ToList());
        }

        public Result TryBuyRoom(RoomProperties roomProporties, Room.Model room)
        {
            Result result = financesController.TryTakeMoney(roomProporties.Cost);
            if (result.Success)
            {
                Inventory.Room.Model inventory_room = room.GetComponent<Inventory.Room.Model>();
                inventoryController.AddNewRoom(inventory_room);
                return new SuccessResult();
            }
            else
            {
                // TODO show something
                Debug.Log(result.Error);
                return result;
            }
        }

        public void SetShopEmployees(IEnumerable<EmployeeConfig> employee_configs)
        {
            // TODO
        }

        public bool TryBuyEmployee(int cost, Room.View tile_ui)
        {
            // TODO
            throw new System.NotImplementedException();
        }
    }
}
