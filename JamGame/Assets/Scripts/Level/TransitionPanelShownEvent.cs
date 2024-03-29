using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    [AddComponentMenu("Scripts/Level/Level.TransitionPanelShownEvent")]
    public class TransitionPanelShownEvent : MonoBehaviour
    {
        public UnityEvent OnShown;

        public void Shown()
        {
            OnShown?.Invoke();
        }
    }
}
