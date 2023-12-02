using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace Level.TransitionPanel
{
    [AddComponentMenu("Scripts/Level.TransitionPanel.Model")]
    public class Model : MonoBehaviour
    {
        [ReadOnly]
        [SerializeField]
        private string panelText;
        public UnityEvent<string> TextChange;
        public string PanelText
        {
            get => panelText;
            set
            {
                panelText = value;
                TextChange?.Invoke(panelText);
            }
        }
    }
}
