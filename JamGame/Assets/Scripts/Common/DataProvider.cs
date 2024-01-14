using Scripts;
using System;

namespace Common
{
    public interface IDataProvider { }

    public class DataProvider<D> : IDataProvider
    {
        Func<D> fetchData;

        public DataProvider(Func<D> fetch_data)
        {
            fetchData = fetch_data;
            DataProviderServiceLocator.RegisterProvider(this);
        }

        ~DataProvider()
        {
            DataProviderServiceLocator.UnregisterProvider(this);
        }

        public D GetData()
        {
            return fetchData();
        }
    }
}
