using UnityEngine;

namespace Overlay
{
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private SerializedOverlay overlay;

        [SerializeField]
        private OverlaySelectorProxy proxy;

        public void Activate()
        {
            proxy.ActivateOverlay(overlay.ToOverlay());
        }
    }
}
