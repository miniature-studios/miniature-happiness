using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Level.Room;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Level.Shop.Room
{
    [AddComponentMenu("Scripts/Level/Shop/Room/Level.Shop.Room.CardView")]
    public class CardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Required]
        [SerializeField]
        private TMP_Text nameLabel;

        [Required]
        [SerializeField]
        private Image image;

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

        [ReadOnly]
        [SerializeField]
        private Controller shopController;

        public bool IsEmpty => coreModels.Count == 0;
        public int Amount => coreModels.Count;
        public CoreModel CoreModel => coreModels.Last();
        public InternalUid Uid => CoreModel.Uid;

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
            image.sprite = CoreModel.ShopModel.CardSprite;
            nameLabel.text = CoreModel.RoomInfo.Title;
            costLabel.text = CoreModel.ShopModel.Cost.Value.ToString();
            countLabel.text = Amount.ToString();
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
