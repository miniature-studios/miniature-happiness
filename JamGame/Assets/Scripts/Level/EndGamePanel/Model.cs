using Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Level.EndGamePanel
{
    [AddComponentMenu("Scripts/Level.EndGamePanel.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField]
        [InspectorReadOnly]
        private string panelText;

        [SerializeField]
        [InspectorReadOnly]
        private bool shown;

        [SerializeField]
        private string loadingScene;

        public UnityEvent<string> TextChange;
        public UnityEvent<bool> ShownTrigger;

        public string PanelText
        {
            get => panelText;
            set
            {
                panelText = value;
                TextChange?.Invoke(panelText);
            }
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
