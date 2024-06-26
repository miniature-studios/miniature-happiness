﻿using Level.Finances;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Level.WinGamePanel
{
    [AddComponentMenu("Scripts/Level/WinGamePanel/Level.WinGamePanel.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private string loadingScene;

        public DaysLived DaysLived =>
            DataProviderServiceLocator.FetchDataFromSingleton<DaysLived>();

        public Money MoneyEarned => DataProviderServiceLocator.FetchDataFromSingleton<Money>();

        public UnityEvent<Model> OnModelChanged;

        // Called by event from animation.
        public void Showing()
        {
            OnModelChanged?.Invoke(this);
        }

        // Called by button Try again on WinGamePanel.
        public void TryAgainClick()
        {
            SceneManager.LoadScene(loadingScene, LoadSceneMode.Single);
        }
    }
}
