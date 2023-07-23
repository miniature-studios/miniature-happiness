namespace Common
{
    // TODO: When stats collection, logging, e.t.c will be implemented, implement
    // some service locator to find all the needed IDataProvider's. 
    public interface IDataProvider<D>
    {
        D GetData();
    }
}