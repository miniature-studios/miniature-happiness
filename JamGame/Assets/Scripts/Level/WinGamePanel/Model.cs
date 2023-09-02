using Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Level.WinGamePanel
{
    [AddComponentMenu("Scripts/Level.WinGamePanel.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        [InspectorReadOnly]
        private bool shown;

        [SerializeField]
        private string loadingScene;

        public UnityEvent<bool> ShownTrigger;

        [SerializeField]
        [InspectorReadOnly]
        private int daysLived;

        public int DaysLived
        {
            get => daysLived;
            set => daysLived = value;
        }

        [SerializeField]
        [InspectorReadOnly]
        private int moneyEarned;

        public int MoneyEarned
        {
            get => moneyEarned;
            set => moneyEarned = value;
        }

        public bool Shown
        {
            get => shown;
            set
            {
                shown = value;
                ShownTrigger?.Invoke(shown);
            }
        }

        public void TryAgainClick()
        {
            SceneManager.LoadScene(loadingScene);
        }
    }
}
