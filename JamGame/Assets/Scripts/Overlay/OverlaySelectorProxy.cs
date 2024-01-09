using UnityEngine;

namespace Overlay
{
    // Used to proxy calls from concrete overlay buttons to Location.
    [AddComponentMenu("Scripts/Overlay/Overlay.OverlaySelectorProxy")]
    public class OverlaySelectorProxy : MonoBehaviour
    {
        [SerializeField]
        private Manager overlayManager;

        public void ActivateOverlay(IOverlay overlay)
        {
            overlay.Activate(overlayManager);
        }
    }
}
