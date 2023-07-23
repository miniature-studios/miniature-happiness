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
        public Model Model => model;

        private RoomProperties roomProperties;
        private Func<RoomProperties, Model, Result> roomBuying;

        private void OnValidate()
        {
            UpdateView();
        }

        private void Awake()
        {
            roomBuying = GetComponentInParent<Controller>().TryBuyRoom;
            UpdateView();
        }

        private void UpdateView()
        {
            if (
                model.TileUnion.TryGetComponent(out roomProperties)
                && model.TileUnion.TryGetComponent(out RoomViewProperties roomViewProperties)
            )
            {
                roomNameText.text = roomViewProperties.RoomName;
                moneyText.text = "Money cost: " + Convert.ToString(roomProperties.Cost);
                waterText.text = "Water: " + Convert.ToString(roomProperties.WaterConsumption);
                electricityText.text =
                    "Electro: " + Convert.ToString(roomProperties.ElectricityComsumption);
            }
            else
            {
                Debug.LogError($"No room properties in {model.name}");
            }
        }

        public void TryBuyRoom()
        {
            if (roomBuying(roomProperties, model).Success)
            {
                Destroy(gameObject);
            }
        }
    }
}
