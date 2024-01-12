using Sirenix.OdinInspector;
using UnityEngine;

namespace Overlay
{
    [AddComponentMenu("Scripts/Overlay/Overlay.Controller")]
    public class Controller : MonoBehaviour
    {
        [HideLabel]
        [InlineProperty]
        [SerializeReference]
        private IOverlay overlay;

        [SerializeField]
        [SceneObjectsOnly]
        private OverlaySelectorProxy proxy;

        public void Activate()
        {
            proxy.ActivateOverlay(overlay);
        }
    }
}
