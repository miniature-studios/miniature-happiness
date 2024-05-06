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
        [ValidateInput(
            nameof(ValidateOverlays),
            "Not all GameObjects contain IOverlayRenderer component."
        )]
        private GameObject[] staticOverlayRenderersHolder;

        private IOverlayRenderer[] dynamicOverlayRenderers;
        private HashSet<IOverlayRenderer> staticOverlayRenderers = new();

        private IEnumerable<IOverlayRenderer> OverlayRenderers =>
            dynamicOverlayRenderers.Concat(staticOverlayRenderers);

        private bool ValidateOverlays()
        {
            return staticOverlayRenderersHolder.All(x =>
                x.GetComponent<IOverlayRenderer>() != null
            );
        }

        private void Start()
        {
            dynamicOverlayRenderers = GetComponentsInChildren<IOverlayRenderer>();
            foreach (GameObject gameObject in staticOverlayRenderersHolder)
            {
                IOverlayRenderer overlayRenderer = gameObject.GetComponent<IOverlayRenderer>();
                if (!staticOverlayRenderers.Add(overlayRenderer))
                {
                    Debug.LogError($"Duplicated IOverlayRenderer reference in {gameObject.name}");
                }
            }
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
