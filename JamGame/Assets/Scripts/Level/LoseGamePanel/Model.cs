using Level.Finances;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Level.LoseGamePanel
{
    [AddComponentMenu("Scripts/Level/LoseGamePanel/Level.LoseGamePanel.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private string loadingScene;

        public DaysLived DaysLived =>
            DataProviderServiceLocator.FetchDataFromSingleton<DaysLived>();

        public MoneyEarned MoneyEarned =>
            DataProviderServiceLocator.FetchDataFromSingleton<MoneyEarned>();

        public UnityEvent<Model> OnModelChanged;

        // Called by event from animation.
        public void Showing()
        {
            OnModelChanged?.Invoke(this);
        }

        // Called by button Try again on WinGamePanel.
        public void TryAgainClick()
        {
            SceneManager.LoadScene(loadingScene);
        }
    }
}
