using Sirenix.OdinInspector;
using UnityEngine;

namespace Overlay
{
    [AddComponentMenu("Scripts/Overlay.Controller")]
    public class Controller : SerializedMonoBehaviour
    {
        [SerializeField]
        private IOverlay overlay;

        [SerializeField]
        private OverlaySelectorProxy proxy;

        public void Activate()
        {
            proxy.ActivateOverlay(overlay);
        }
    }
}
