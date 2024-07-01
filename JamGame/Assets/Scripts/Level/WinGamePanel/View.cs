using TMPro;
using UnityEngine;

namespace Level.WinGamePanel
{
    [AddComponentMenu("Scripts/Level/WinGamePanel/Level.WinGamePanel.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text infoLabel;

        public void OnModelChanged(Model model)
        {
            infoLabel.text = $"Days - {model.DaysLived.Value} || Money - {model.MoneyEarned.Value}";
        }
    }
}
