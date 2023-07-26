using Common;
using System;
using TMPro;
using UnityEngine;

namespace Level.Shop.Room
{
    [AddComponentMenu("Level.Shop.Room.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private Model model;

        [SerializeField]
        private TMP_Text moneyText;

        [SerializeField]
        private TMP_Text waterText;

        [SerializeField]
        private TMP_Text electricityText;

        [SerializeField]
        private TMP_Text roomNameText;

        private TileUnion.Cost roomCost;
        private Func<TileUnion.Cost, Model, Result> roomBuying;

        private void Awake()
        {
            roomBuying = GetComponentInParent<Controller>().TryBuyRoom;
            moneyText.text =
                "Money cost: " + Convert.ToString(model.InventoryRoomModel.TileUnion.Cost.Value);
            waterText.text =
                "Water: "
                + Convert.ToString(
                    model.InventoryRoomModel.TileUnion.TarrifProperties.WaterConsumption
                );
            electricityText.text =
                "Electro: "
                + Convert.ToString(
                    model.InventoryRoomModel.TileUnion.TarrifProperties.ElectricityConsumption
                );
        }

        public void TryBuyRoom()
        {
            if (roomBuying(roomCost, model).Success)
            {
                Destroy(gameObject);
            }
        }
    }
}
