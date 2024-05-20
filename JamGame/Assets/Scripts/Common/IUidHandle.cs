namespace Common
{
    public interface IUidHandle
    {
        public InternalUid Uid { get; }
    }

    public interface IPostprocessedUidHandle
    {
        public InternalUid Uid { get; }
    }
}
