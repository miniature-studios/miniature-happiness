using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Overlay
{
    public interface IOverlayManager
    {
        public void RevertAllOverlays();

        public void ApplyOverlay<O>(O overlay)
            where O : class, IOverlay;
    }

    [AddComponentMenu("Overlay.Manager")]
    public class Manager : MonoBehaviour, IOverlayManager
    {
        // TODO: Update in runtime.
        private List<IOverlayRenderer> overlayRenderers;

        private void Start()
        {
            overlayRenderers = GetComponentsInChildren<IOverlayRenderer>().ToList();
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
