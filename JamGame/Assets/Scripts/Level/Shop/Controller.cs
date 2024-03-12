using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Config;
using Level.Inventory.Controller;
using Level.Room;
using Location;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Shop
{
    [AddComponentMenu("Scripts/Level/Shop/Level.Shop.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private ControllerImpl inventoryController;

        [Required]
        [SerializeField]
        private Model shopModel;

        [SerializeField]
        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Finances.Model financesController;

        [SerializeField]
        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private EmployeeManager employeeManager;

        public void SetShopRooms(IEnumerable<ShopRoomConfig> roomConfigs)
        {
            IEnumerable<CoreModel> newCoreModels = roomConfigs.Select(x =>
                CoreModel.InstantiateCoreModel(x.Room.Uid)
            );
            shopModel.ResetRooms(newCoreModels);
        }

        public Result TryBuyRoom(InternalUid roomUid, int cost)
        {
            Result takeMoneyResult = financesController.TryTakeMoney(cost);
            if (takeMoneyResult.Failure)
            {
                // TODO show something
                Debug.Log(takeMoneyResult.Error);
                return new FailResult(takeMoneyResult.Error);
            }

            Result<CoreModel> borrowedResult = shopModel.BorrowRoom(roomUid);
            if (borrowedResult.Failure)
            {
                return new FailResult(borrowedResult.Error);
            }

            inventoryController.AddNewRoom(borrowedResult.Data);
            return new SuccessResult();
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
                EmployeeConfig borrowedEmployee = shopModel.BorrowEmployee(employee);
                result = employeeManager.AddEmployee(borrowedEmployee);
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
