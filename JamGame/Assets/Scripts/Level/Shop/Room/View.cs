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
        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel CoreModelPrefab;

        [SerializeField]
        [InspectorReadOnly]
        private string hashCode;
        public string HashCode
        {
            get => hashCode;
            set => hashCode = value;
        }

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
                "Money cost: " + Convert.ToString(GetCoreModelInstance().ShopModel.Cost.Value);
            waterText.text =
                "Water: "
                + Convert.ToString(GetCoreModelInstance().TariffProperties.WaterConsumption);
            electricityText.text =
                "Electro: "
                + Convert.ToString(GetCoreModelInstance().TariffProperties.ElectricityConsumption);
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
