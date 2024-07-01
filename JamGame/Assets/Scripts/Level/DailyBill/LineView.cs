using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Level.DailyBill
{
    [AddComponentMenu("Scripts/Level/DailyBill/Level.DailyBill.LineView")]
    internal class LineView : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private Image icon;

        [Required]
        [SerializeField]
        private TMP_Text nameLabel;

        [Required]
        [SerializeField]
        private TMP_Text costLabel;

        [Required]
        [SerializeField]
        private TMP_Text countLabel;

        [Required]
        [SerializeField]
        private TMP_Text sumLabel;

        public void FillWithData(RoomCheck roomCheck)
        {
            icon.sprite = roomCheck.CoreModel.ShopModel.CardSprite;
            nameLabel.text = roomCheck.CoreModel.RoomInfo.Title;
            costLabel.text = roomCheck.OneCost.ToString() + "$";
            countLabel.text = "X" + roomCheck.Count.ToString();
            sumLabel.text = "-" + roomCheck.SumCost.ToString();
        }
    }
}
