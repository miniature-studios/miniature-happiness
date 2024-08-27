using System.Collections.Generic;
using System.Linq;
using Common;
using Employee.Personality;
using Level.Config;
using Level.Inventory.Controller;
using Level.Room;
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
        private Location.EmployeeManager.Model employeeManager;

        [SerializeField]
        private InfoPopup notEnoughMoneyPopup;

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
                notEnoughMoneyPopup.Show();
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

        public void SetShopEmployees(IEnumerable<PersonalityImpl> employeePersonalities)
        {
            shopModel.ResetEmployees(employeePersonalities);
        }

        public Result TryBuyEmployee(PersonalityImpl employee)
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
                notEnoughMoneyPopup.Show();
                return result;
            }
        }
    }
}
