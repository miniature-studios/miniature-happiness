public interface IOverlayRenderer
{
    public void RevertOverlays();
}

public interface IOverlayRenderer<T> : IOverlayRenderer
    where T : class, IOverlay
{
    void ApplyOverlay(T overlay);
}
