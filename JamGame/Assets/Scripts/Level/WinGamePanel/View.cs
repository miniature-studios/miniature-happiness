using TMPro;
using UnityEngine;

namespace Level.WinGamePanel
{
    [AddComponentMenu("Scripts/Level.WinGamePanel.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text daysLabel;

        [SerializeField]
        private TMP_Text moneyLabel;

        public void OnModelChanged(Model model)
        {
            daysLabel.text = $"Days lived: {model.DaysLived.Value.Days_}";
            moneyLabel.text = $"Money earned: {model.MoneyEarned.Value} Coins";
        }
    }
}
