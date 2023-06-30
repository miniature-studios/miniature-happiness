using Overlay;
using UnityEngine;

public class OverlayButton : MonoBehaviour
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
