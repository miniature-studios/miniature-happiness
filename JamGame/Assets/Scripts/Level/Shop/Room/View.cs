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

        public string HashCode => CoreModelPrefab.Uid;

        [SerializeField]
        private TMP_Text moneyText;

        [SerializeField]
        private TMP_Text waterText;

        [SerializeField]
        private TMP_Text electricityText;

        private Func<CoreModel, Result> roomBuying;

        [SerializeField]
        [InspectorReadOnly]
        private CoreModel coreModel;
        public CoreModel CoreModel => coreModel;

        public void SetCoreModel(CoreModel coreModel)
        {
            this.coreModel = coreModel;
        }

        private void Awake()
        {
            roomBuying = GetComponentInParent<Controller>().TryBuyRoom;
        }

        private void Update()
        {
            moneyText.text = $"Money cost: {CoreModel.ShopModel.Cost.Value}";
            waterText.text = $"Water: {CoreModel.TariffProperties.WaterConsumption}";
            electricityText.text = $"Electro: {CoreModel.TariffProperties.ElectricityConsumption}";
        }

        // Called be pressing button
        public void TryBuyRoom()
        {
            if (roomBuying(CoreModel).Success)
            {
                Destroy(gameObject);
            }
        }
    }
}
