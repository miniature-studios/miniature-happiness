namespace Common
{
    public interface IUidHandle
    {
        public InternalUid Uid { get; }
    }

    public interface IUidPostprocessingHandle
    {
        public InternalUid Uid { get; }
    }
}
