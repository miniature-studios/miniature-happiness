namespace Overlay
{
    public interface IOverlayRenderer
    {
        // TODO: #171
        public void RevertOverlays();
    }

    public interface IOverlayRenderer<T> : IOverlayRenderer
        where T : class, IOverlay
    {
        void ApplyOverlay(T overlay);
    }
}
