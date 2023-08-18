using Common;
using Level.Room;
using System;
using TMPro;
using UnityEngine;

namespace Level.Shop.Room
{
    [AddComponentMenu("Scripts/Level.Shop.Room.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private UniqueId coreModelUniqueId;
        public UniqueId CoreModelUniqueId => coreModelUniqueId;

        [SerializeField]
        private TMP_Text moneyText;

        [SerializeField]
        private TMP_Text waterText;

        [SerializeField]
        private TMP_Text electricityText;

        private Func<Cost, CoreModel, Result> roomBuying;
        private Func<Cost> getCost = null;
        private Func<TariffProperties> getTariff = null;
        public Func<CoreModel> GetCoreModelInstance = null;

        private void Awake()
        {
            roomBuying = GetComponentInParent<Controller>().TryBuyRoom;
        }

        public void Constructor(
            Func<Cost> getCost,
            Func<TariffProperties> getTariff,
            Func<CoreModel> getCoreModelInstance
        )
        {
            this.getCost = getCost;
            this.getTariff = getTariff;
            GetCoreModelInstance = getCoreModelInstance;
        }

        private void Update()
        {
            if (getCost != null && getTariff != null)
            {
                moneyText.text = "Money cost: " + Convert.ToString(getCost().Value);
                waterText.text = "Water: " + Convert.ToString(getTariff().WaterConsumption);
                electricityText.text =
                    "Electro: " + Convert.ToString(getTariff().ElectricityConsumption);
            }
        }

        // Called be pressing button
        public void TryBuyRoom()
        {
            if (roomBuying(getCost(), GetCoreModelInstance()).Success)
            {
                Destroy(gameObject);
            }
        }
    }
}
