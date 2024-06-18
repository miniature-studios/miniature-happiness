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
        private TMP_Text infoLabel;

        [SerializeField]
        private List<LabelByCause> causeLabels;

        public void OnModelChanged(Model model)
        {
            infoLabel.text =
                $"Days - {model.DaysLived.Value} || Cause - {causeLabels.Find(x => x.Cause == model.LoseCause).Label}";
        }
    }
}
