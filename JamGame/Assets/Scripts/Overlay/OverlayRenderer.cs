public interface IOverlayRenderer { }

public interface IOverlayRenderer<T> : IOverlayRenderer
    where T : class, IOverlay
{
    void ApplyOverlay(T overlay);
    void RevertOverlay(T overlay);
}