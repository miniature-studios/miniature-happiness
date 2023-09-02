using Common;
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
        [InspectorReadOnly]
        private int daysLived;

        public int DaysLived
        {
            get => daysLived;
            set
            {
                daysLived = value;
                OnModelChanged?.Invoke(this);
            }
        }

        [SerializeField]
        [InspectorReadOnly]
        private int moneyEarned;

        public int MoneyEarned
        {
            get => moneyEarned;
            set
            {
                moneyEarned = value;
                OnModelChanged?.Invoke(this);
            }
        }

        public UnityEvent<Model> OnModelChanged;

        // Called by button Try again on LoseGamePanel.
        public void TryAgainClick()
        {
            SceneManager.LoadScene(loadingScene);
        }
    }
}
