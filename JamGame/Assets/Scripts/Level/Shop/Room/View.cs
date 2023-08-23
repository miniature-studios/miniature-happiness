using Common;
using Level.Room;
using Pickle;
using System;
using TMPro;
using UnityEngine;

namespace Level.Shop.Room
{
    [AddComponentMenu("Scripts/Level.Shop.Room.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        [Pickle(LookupType = ObjectProviderType.Assets)]
        private CoreModel coreModel;

        public CoreModel CoreModel => coreModel;

        [SerializeField]
        private TMP_Text moneyText;

        [SerializeField]
        private TMP_Text waterText;

        [SerializeField]
        private TMP_Text electricityText;

        private Func<CoreModel, Result> roomBuying;
        public Func<CoreModel> GetCoreModelInstance = null;

        private void Awake()
        {
            roomBuying = GetComponentInParent<Controller>().TryBuyRoom;
        }

        public void Constructor(Func<CoreModel> getCoreModelInstance)
        {
            GetCoreModelInstance = getCoreModelInstance;
        }

        private void Update()
        {
            moneyText.text =
                "Money cost: "
                + Convert.ToString(GetCoreModelInstance().RoomInformation.Cost.Value);
            waterText.text =
                "Water: "
                + Convert.ToString(
                    GetCoreModelInstance().RoomInformation.TariffProperties.WaterConsumption
                );
            electricityText.text =
                "Electro: "
                + Convert.ToString(
                    GetCoreModelInstance().RoomInformation.TariffProperties.ElectricityConsumption
                );
        }

        // Called be pressing button
        public void TryBuyRoom()
        {
            if (roomBuying(GetCoreModelInstance()).Success)
            {
                Destroy(gameObject);
            }
        }
    }
}
