using System;
using System.Collections.Generic;
using Level.Config;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Level.LoseGamePanel
{
    [AddComponentMenu("Scripts/Level/LoseGamePanel/Level.LoseGamePanel.View")]
    public class View : MonoBehaviour
    {
        [Serializable]
        private struct LabelByCause
        {
            public LoseGame.Cause Cause;
            public string Label;
        }

        [Required]
        [SerializeField]
        private TMP_Text daysLabel;

        [Required]
        [SerializeField]
        private TMP_Text causeLabel;

        [SerializeField]
        private List<LabelByCause> causeLabels;

        public void OnModelChanged(Model model)
        {
            daysLabel.text = $"Days lived: {model.DaysLived.Value}";
            causeLabel.text =
                $"Lose cause: {causeLabels.Find(x => x.Cause == model.LoseCause).Label}";
        }
    }
}
