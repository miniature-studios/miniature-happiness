using TMPro;
using UnityEngine;

namespace Level.TransitionPanel
{
    [AddComponentMenu("Scripts/Level.TransitionPanel.View")]
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
