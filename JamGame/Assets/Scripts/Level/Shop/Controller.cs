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
        private Inventory.Controller inventoryController;

        [SerializeField]
        private Model shopModel;

        [SerializeField]
        private Finances.Model financesController;

        public void SetShopRooms(IEnumerable<ShopRoomConfig> room_configs)
        {
            shopModel.SetRooms(room_configs.Select(x => x.Room).ToList());
        }

        public Result TryBuyRoom(TileUnion.Cost cost, Room.Model room)
        {
            Result result = financesController.TryTakeMoney(cost.Value);
            if (result.Success)
            {
                inventoryController.AddNewRoom(room.InventoryRoomModel);
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
