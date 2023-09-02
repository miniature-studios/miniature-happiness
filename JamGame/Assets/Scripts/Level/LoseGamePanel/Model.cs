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
        [InspectorReadOnly]
        private bool shown;

        [SerializeField]
        private string loadingScene;

        public UnityEvent<bool> ShownTrigger;

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
