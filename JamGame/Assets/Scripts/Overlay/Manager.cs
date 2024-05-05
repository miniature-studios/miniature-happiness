using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
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
        [SerializeField]
        [ValidateInput(nameof(ValidateOverlays))]
        private GameObject[] staticOverlayRenderers;

        private IOverlayRenderer[] dynamicOverlayRenderers;

        private IEnumerable<IOverlayRenderer> OverlayRenderers =>
            dynamicOverlayRenderers.Concat(
                staticOverlayRenderers.Select(x => x.GetComponent<IOverlayRenderer>())
            );

        private bool ValidateOverlays()
        {
            return staticOverlayRenderers.All(x => x.GetComponent<IOverlayRenderer>() != null);
        }

        private void Start()
        {
            dynamicOverlayRenderers = GetComponentsInChildren<IOverlayRenderer>();
        }

        private void Update()
        {
            // TODO: #174
            dynamicOverlayRenderers = GetComponentsInChildren<IOverlayRenderer>();
        }

        // TODO: #174
        public void RevertAllOverlays()
        {
            foreach (IOverlayRenderer overlay_renderer in OverlayRenderers)
            {
                overlay_renderer.RevertOverlays();
            }
        }

        public void ApplyOverlay<O>(O overlay)
            where O : class, IOverlay
        {
            foreach (IOverlayRenderer overlay_renderer in OverlayRenderers)
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
