using TMPro;
using UnityEngine;

namespace Level.LoseGamePanel
{
    [AddComponentMenu("Scripts/Level.LoseGamePanel.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text daysLabel;

        [SerializeField]
        private TMP_Text moneyLabel;

        public void OnModelChanged(Model model)
        {
            daysLabel.text = $"Days lived: {model.DaysLived}";
            moneyLabel.text = $"Days lived: {model.MoneyEarned}";
        }
    }
}
