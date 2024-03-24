using System;

namespace Common
{
    public interface IDataProvider { }

    public class DataProvider<D> : IDataProvider
    {
        private Func<D> fetchData;

        public DataProvider(Func<D> fetch_data, DataProviderServiceLocator.ResolveType resolve_type)
        {
            fetchData = fetch_data;
            DataProviderServiceLocator.Register(this, resolve_type);
        }

        ~DataProvider()
        {
            DataProviderServiceLocator.Unregister(this);
        }

        public D GetData()
        {
            return fetchData();
        }
    }
}
