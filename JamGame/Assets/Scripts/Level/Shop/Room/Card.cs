using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Level.Shop.Room
{
    [AddComponentMenu("Scripts/Level/Shop/Room/Level.Shop.Room.Card")]
    public class Card : MonoBehaviour
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

        public void UpdateData(Plank plank)
        {
            title.text = plank.CoreModel.RoomInfo.Title;
            description.text = plank.CoreModel.RoomInfo.Description;
            image.sprite = plank.CoreModel.ShopModel.CardSprite;
            costLabel.text = plank.CoreModel.ShopModel.Cost.Value.ToString();
            rentLabel.text = $"{plank.CoreModel.RoomInfo.RentCost.Value}/day";
            quantityLabel.text = plank.RoomQuantity.ToString();
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
