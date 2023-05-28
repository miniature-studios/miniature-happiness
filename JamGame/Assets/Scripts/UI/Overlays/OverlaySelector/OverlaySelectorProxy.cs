using UnityEngine;

// Used to proxy calls from concrete overlay buttons to Location.
public class OverlaySelectorProxy : MonoBehaviour
{
    [SerializeField] OverlayManager overlayManager;

    public void ActivateOverlay(IOverlay overlay)
    {
        overlay.Activate(overlayManager);
    }
}
