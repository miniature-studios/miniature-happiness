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

    [AddComponentMenu("Scripts/Overlay/Overlay.Manager")]
    public class Manager : MonoBehaviour, IOverlayManager
    {
        private List<IOverlayRenderer> overlayRenderers;

        private void Start()
        {
            overlayRenderers = GetComponentsInChildren<IOverlayRenderer>().ToList();
        }

        private void Update()
        {
            // TODO: #174
            overlayRenderers = GetComponentsInChildren<IOverlayRenderer>().ToList();
        }

        // TODO: #174
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
