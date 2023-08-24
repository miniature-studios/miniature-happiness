﻿using Common;
using Level.Config;
using Level.Room;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Level.Shop
{
    [AddComponentMenu("Scripts/Level.Shop.Controller")]
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
            shopModel.ResetRooms(
                room_configs.Select(
                    x => CoreModelsManager.Instance.InstantiateCoreModel(x.Room.HashCode)
                )
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

        public void SetShopEmployees(IEnumerable<EmployeeConfig> employee_configs)
        {
            // TODO
        }

        public bool TryBuyEmployee(int cost)
        {
            // TODO
            throw new System.NotImplementedException();
        }
    }
}
