using TMPro;
using UnityEngine;

namespace Level.WinGamePanel
{
    [AddComponentMenu("Scripts/Level.WinGamePanel.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;

        [SerializeField]
        private TMP_Text daysLabel;

        [SerializeField]
        private TMP_Text moneyLabel;

        [SerializeField]
        private Model model;

        public void ShownTrigger(bool shown)
        {
            animator.SetBool("Shown", shown);
            daysLabel.text = $"Days lived: {model.DaysLived}";
            moneyLabel.text = $"Days lived: {model.MoneyEarned}";
        }
    }
}
