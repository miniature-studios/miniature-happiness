using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Level.Shop.Room
{
    [AddComponentMenu("Scripts/Level/Shop/Room/Level.Shop.Room.Card")]
    public class Card : MonoBehaviour
    {
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
            costLabel.text = plank.CoreModel.ShopModel.Cost.ToString();
            rentLabel.text = $"{plank.CoreModel.RoomInfo.RentCost}/day";
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
