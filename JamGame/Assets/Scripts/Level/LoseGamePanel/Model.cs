using Common;
using Level.Finances;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Level.LoseGamePanel
{
    [AddComponentMenu("Scripts/Level.LoseGamePanel.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private string loadingScene;

        [SerializeField]
        private IDataProvider<DaysLived> daysLived;
        public DaysLived DaysLived => daysLived.GetData();

        [SerializeField]
        private IDataProvider<MoneyEarned> moneyEarned;
        public MoneyEarned MoneyEarned => moneyEarned.GetData();

        public UnityEvent<Model> OnModelChanged;

        public void PrepareToShow()
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
