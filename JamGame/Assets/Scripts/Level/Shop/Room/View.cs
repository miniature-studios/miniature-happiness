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

        public string Uid => CoreModelPrefab.Uid;

        [SerializeField]
        private TMP_Text costText;

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
            costText.text = $"Money cost: {CoreModel.ShopModel.Cost.Value}";
        }

        // Called by pressing button.
        public void TryBuyRoom()
        {
            if (roomBuying(CoreModel).Success)
            {
                Destroy(gameObject);
            }
        }
    }
}
