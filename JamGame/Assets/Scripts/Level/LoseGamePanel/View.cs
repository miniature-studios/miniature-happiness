using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Level.LoseGamePanel
{
    [AddComponentMenu("Scripts/Level/LoseGamePanel/Level.LoseGamePanel.View")]
    public class View : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private TMP_Text daysLabel;

        [Required]
        [SerializeField]
        private TMP_Text causeLabel;

        public void OnModelChanged(Model model)
        {
            daysLabel.text = $"Days lived: {model.DaysLived.Value}";
            causeLabel.text = $"Lose cause: {model.LoseCause}";
        }
    }
}
