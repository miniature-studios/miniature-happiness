using Common;
using Level.Finances;
using Level.GlobalTime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Level.WinGamePanel
{
    [AddComponentMenu("Scripts/Level.WinGamePanel.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        private string loadingScene;

        [SerializeField]
        [Pickle(typeof(IDataProvider<DaysLived>), LookupType = Pickle.ObjectProviderType.Scene)]
        private MonoBehaviour daysLivedDataProvider;
        public DaysLived DaysLived => (daysLivedDataProvider as IDataProvider<DaysLived>).GetData();

        [SerializeField]
        [Pickle(typeof(IDataProvider<MoneyEarned>), LookupType = Pickle.ObjectProviderType.Scene)]
        private MonoBehaviour moneyEarnedDataProvider;
        public MoneyEarned MoneyEarned =>
            (moneyEarnedDataProvider as IDataProvider<MoneyEarned>).GetData();

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
