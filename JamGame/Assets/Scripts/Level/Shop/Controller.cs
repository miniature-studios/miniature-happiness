﻿using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Config;
using Level.Inventory.Controller;
using Level.Room;
using Location;
using UnityEngine;

namespace Level.Shop
{
    [AddComponentMenu("Scripts/Level/Shop/Level.Shop.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private ControllerImpl inventoryController;

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
                CoreModel borrowedRoom = shopModel.BorrowRoom(room);
                inventoryController.AddNewRoom(borrowedRoom);
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
