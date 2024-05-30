using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Overlay
{
    [AddComponentMenu("Scripts/Overlay/Overlay.Controller")]
    public class Controller : MonoBehaviour
    {
        [HideLabel]
        [InlineProperty]
        [SerializeReference]
        private IOverlay overlay;

        [Required]
        [SerializeField]
        private OverlaySelectorProxy proxy;

        [Required]
        [SerializeField]
        private GameObject selectedIcon;

        [Required]
        [SerializeField]
        private Toggle toggle;

        // Called by toggle event.
        public void StateChanged(bool state)
        {
            selectedIcon.SetActive(state);

            if (state)
            {
                proxy.ActivateOverlay(overlay);
            }
        }
    }
}
