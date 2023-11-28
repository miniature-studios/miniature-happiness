using UnityEngine;

namespace Overlay
{
    [AddComponentMenu("Scripts/Overlay.Controller")]
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
