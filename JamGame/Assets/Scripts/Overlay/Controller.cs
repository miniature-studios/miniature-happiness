using UnityEngine;

namespace Overlay
{
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private readonly SerializedOverlay overlay;

        [SerializeField]
        private readonly OverlaySelectorProxy proxy;

        public void Activate()
        {
            proxy.ActivateOverlay(overlay.ToOverlay());
        }
    }
}
