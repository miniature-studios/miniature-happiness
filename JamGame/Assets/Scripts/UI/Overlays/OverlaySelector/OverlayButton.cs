using UnityEngine;

public class OverlayButton : MonoBehaviour
{
    [SerializeField] SerializedOverlay overlay;
    [SerializeField] OverlaySelectorProxy proxy;

    public void Activate()
    {
        proxy.ActivateOverlay(overlay.ToOverlay());
    }
}
