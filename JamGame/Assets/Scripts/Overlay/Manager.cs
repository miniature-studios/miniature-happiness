using UnityEngine;

namespace Overlay
{
    public interface IOverlayManager
    {
        public void RevertAllOverlays();

        public void ApplyOverlay<O>(O overlay)
            where O : class, IOverlay;
    }

    [AddComponentMenu("Scripts/Overlay/Overlay.Manager")]
    public class Manager : MonoBehaviour, IOverlayManager
    {
        // TODO: Update in runtime.
        private IOverlayRenderer[] overlayRenderers;

        private void Start()
        {
            overlayRenderers = GetComponentsInChildren<IOverlayRenderer>();
        }

        private void Update()
        {
            // TODO: Find optimal approach.
            overlayRenderers = GetComponentsInChildren<IOverlayRenderer>();
        }

        public void RevertAllOverlays()
        {
            foreach (IOverlayRenderer overlay_renderer in overlayRenderers)
            {
                overlay_renderer.RevertOverlays();
            }
        }

        public void ApplyOverlay<O>(O overlay)
            where O : class, IOverlay
        {
            foreach (IOverlayRenderer overlay_renderer in overlayRenderers)
            {
                overlay_renderer.RevertOverlays();
                if (overlay_renderer is IOverlayRenderer<O> or)
                {
                    or.ApplyOverlay(overlay);
                }
            }
        }
    }
}
