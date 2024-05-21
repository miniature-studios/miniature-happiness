using Level.Config;
using Sirenix.OdinInspector;
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

        [ReadOnly]
        [SerializeField]
        private LoseGame.Cause cause;
        public LoseGame.Cause LoseCause => cause;

        public UnityEvent<Model> OnModelChanged;

        public void SetCause(LoseGame.Cause cause)
        {
            this.cause = cause;
        }

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
