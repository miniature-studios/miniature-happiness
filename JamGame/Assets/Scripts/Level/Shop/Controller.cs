using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Config;
using Level.Room;
using Location;
using UnityEngine;

namespace Level.Shop
{
    [AddComponentMenu("Scripts/Level/Shop/Level.Shop.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private Inventory.Controller inventoryController;

        [SerializeField]
        private Model shopModel;

        [SerializeField]
        private Finances.Model financesController;

        [SerializeField]
        private EmployeeManager employeeManager;

        public void SetShopRooms(IEnumerable<ShopRoomConfig> roomConfigs)
        {
            shopModel.ResetRooms(
                roomConfigs.Select(x => CoreModel.InstantiateCoreModel(x.Room.Uid))
            );
        }

        public Result TryBuyRoom(CoreModel room)
        {
            Result result = financesController.TryTakeMoney(room.ShopModel.Cost.Value);
            if (result.Success)
            {
                inventoryController.AddNewRoom(shopModel.BorrowRoom(room));
                return new SuccessResult();
            }
            else
            {
                // TODO show something
                Debug.Log(result.Error);
                return result;
            }
        }

        public void SetShopEmployees(IEnumerable<EmployeeConfig> employeeConfigs)
        {
            shopModel.ResetEmployees(employeeConfigs);
        }

        public Result TryBuyEmployee(EmployeeConfig employee)
        {
            Result result = financesController.TryTakeMoney(employee.HireCost);
            if (result.Success)
            {
                result = employeeManager.AddEmployee(employee);
                if (result.Failure)
                {
                    financesController.AddMoney(employee.HireCost);
                    return result;
                }

                return new SuccessResult();
            }
            else
            {
                // TODO show something
                Debug.Log(result.Error);
                return result;
            }
        }
    }
}
