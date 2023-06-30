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

        public void SetShopRooms(IEnumerable<RoomConfig> room_configs)
        {
            shopModel.SetRooms(room_configs.Select(x => x.RoomShopUI).ToList());
        }

        public Result TryBuyRoom(RoomProperties roomProporties, RoomInventoryUI tile_ui)
        {
            Result result = financesController.TryTakeMoney(roomProporties.Cost);
            if (result.Success)
            {
                inventoryController.AddNewRoom(tile_ui);
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

        public bool TryBuyEmployee(int cost, RoomInventoryUI tile_ui)
        {
            // TODO
            throw new System.NotImplementedException();
        }
    }
}
