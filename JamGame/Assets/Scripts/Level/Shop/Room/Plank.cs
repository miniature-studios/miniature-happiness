using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Room;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Level.Shop.Room
{
    [AddComponentMenu("Scripts/Level/Shop/Room/Level.Shop.Room.Plank")]
    public class Plank : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Required]
        [SerializeField]
        private TMP_Text rentLabel;

        [Required]
        [SerializeField]
        private TMP_Text costLabel;

        [Required]
        [SerializeField]
        private TMP_Text countLabel;

        [ReadOnly]
        [SerializeField]
        private List<CoreModel> coreModels = new();
        public bool IsEmpty => coreModels.Count == 0;
        public int RoomQuantity => coreModels.Count;
        public CoreModel CoreModel => coreModels.Last();
        public InternalUid Uid => CoreModel.Uid;

        [ReadOnly]
        [SerializeField]
        private Controller shopController;

        public event Action OnPointerEnterEvent;
        public event Action OnPointerExitEvent;

        public void Initialize()
        {
            shopController = GetComponentInParent<Controller>(true);
        }

        public void AddCoreModel(CoreModel coreModel)
        {
            coreModels.Add(coreModel);
            coreModel.transform.SetParent(transform);
            UpdateTexts();
        }

        public void RemoveCoreModel(CoreModel coreModel)
        {
            _ = coreModels.Remove(coreModel);
        }

        private void UpdateTexts()
        {
            costLabel.text = CoreModel.ShopModel.Cost.Value.ToString();
            countLabel.text = RoomQuantity.ToString();
            rentLabel.text = $"{CoreModel.RoomInfo.RentCost.Value}/day";
        }

        // Called by pressing button.
        public void TryBuyRoom()
        {
            _ = shopController.TryBuyRoom(CoreModel.Uid, CoreModel.ShopModel.Cost.Value);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterEvent?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExitEvent?.Invoke();
        }
    }
}
