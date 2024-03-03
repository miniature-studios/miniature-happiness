namespace Overlay
{
    public interface IOverlayRenderer
    {
        // TODO: Move this to IOverlayRenderer<T>.
        public void RevertOverlays();
    }

    public interface IOverlayRenderer<T> : IOverlayRenderer
        where T : class, IOverlay
    {
        void ApplyOverlay(T overlay);
    }
}
