using Common;
using UnityEngine;
using UnityEngine.Events;

namespace Level.TransitionPanel
{
    [AddComponentMenu("Level.TransitionPanel.Model")]
    public class Model : MonoBehaviour
    {
        [SerializeField, InspectorReadOnly]
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
