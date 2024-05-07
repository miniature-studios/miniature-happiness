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
        private Image contentImage;

        [Required]
        [SerializeField]
        private TMP_Text rentLabel;

        [Required]
        [SerializeField]
        private TMP_Text costLabel;

        [Required]
        [SerializeField]
        private TMP_Text countLabel;

        [SerializeField]
        private List<Image> backgroundImages;

        [SerializeField]
        private Color notHoveredColor;

        [SerializeField]
        private Color hoveredColor;

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
        public event Action OnDataUpdated;

        public void Initialize()
        {
            shopController = GetComponentInParent<Controller>(true);
        }

        public void AddCoreModel(CoreModel coreModel)
        {
            coreModels.Add(coreModel);
            coreModel.transform.SetParent(transform);
            UpdateData();
        }

        public void RemoveCoreModel(CoreModel coreModel)
        {
            _ = coreModels.Remove(coreModel);
            if (Amount != 0)
            {
                UpdateData();
            }
        }

        private void UpdateData()
        {
            contentImage.sprite = CoreModel.ShopModel.CardSprite;
            nameLabel.text = CoreModel.RoomInfo.Title;
            costLabel.text = CoreModel.ShopModel.Cost.Value.ToString();
            countLabel.text = Amount.ToString();
            rentLabel.text = $"{CoreModel.RoomInfo.RentCost.Value}/day";
            OnDataUpdated?.Invoke();
        }

        // Called by pressing button.
        public void TryBuyRoom()
        {
            _ = shopController.TryBuyRoom(CoreModel.Uid, CoreModel.ShopModel.Cost.Value);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnPointerEnterEvent?.Invoke();
            SetBackgroundImagesColor(hoveredColor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            SetBackgroundImagesColor(notHoveredColor);
            OnPointerExitEvent?.Invoke();
        }

        private void SetBackgroundImagesColor(Color color)
        {
            foreach (Image image in backgroundImages)
            {
                image.color = color;
            }
        }
    }
}
