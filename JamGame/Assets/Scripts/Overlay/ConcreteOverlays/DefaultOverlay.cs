using System;

[Serializable]
public class DefaultOverlay : IOverlay
{
    public void Activate(IOverlayManager overlay_manager)
    {
        overlay_manager.RevertAllOverlays();
    }
}
