using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Level.Shop.Room
{
    [AddComponentMenu("Scripts/Level/Shop/Room/Level.Shop.Room.DescriptionView")]
    public class DescriptionView : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Image image;

        [Required]
        [SerializeField]
        private TMP_Text title;

        [Required]
        [SerializeField]
        private TMP_Text description;

        [Required]
        [SerializeField]
        private TMP_Text costLabel;

        [Required]
        [SerializeField]
        private TMP_Text rentLabel;

        [Required]
        [SerializeField]
        private TMP_Text quantityLabel;

        [ReadOnly]
        [SerializeField]
        private CardView card;

        public void LinkCard(CardView card)
        {
            gameObject.SetActive(true);
            this.card = card;
            card.OnDataUpdated += CardDataUpdated;
            UpdateData();
        }

        private void CardDataUpdated()
        {
            UpdateData();
        }

        public void UnlinkCard()
        {
            gameObject.SetActive(false);
            card.OnDataUpdated -= CardDataUpdated;
        }

        private void UpdateData()
        {
            title.text = card.CoreModel.RoomInfo.Title;
            description.text = card.CoreModel.RoomInfo.Description;
            image.sprite = card.CoreModel.ShopModel.DescriptionViewSprite;
            costLabel.text = card.CoreModel.ShopModel.Cost.Value.ToString();
            rentLabel.text = $"{card.CoreModel.RoomInfo.RentCost.Value}/day";
            quantityLabel.text = card.Amount.ToString();
        }
    }
}
