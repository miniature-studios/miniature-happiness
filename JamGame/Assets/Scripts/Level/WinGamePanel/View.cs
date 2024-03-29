﻿using TMPro;
using UnityEngine;

namespace Level.WinGamePanel
{
    [AddComponentMenu("Scripts/Level/WinGamePanel/Level.WinGamePanel.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text daysLabel;

        [SerializeField]
        private TMP_Text moneyLabel;

        public void OnModelChanged(Model model)
        {
            daysLabel.text = $"Days lived: {model.DaysLived.Value}";
            moneyLabel.text = $"Money earned: {model.MoneyEarned.Value} Coins";
        }
    }
}
