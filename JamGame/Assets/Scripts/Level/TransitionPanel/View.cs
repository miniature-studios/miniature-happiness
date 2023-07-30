using TMPro;
using UnityEngine;

namespace Level.TransitionPanel
{
    [AddComponentMenu("Level.TransitionPanel.View")]
    public class View : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text textOnPanel;

        public void UpdateText(string text)
        {
            textOnPanel.text = text;
        }
    }
}
