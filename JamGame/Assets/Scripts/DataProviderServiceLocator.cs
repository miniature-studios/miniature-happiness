using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    [AddComponentMenu("Scripts/Scripts/DataProviderServiceLocator")]
    public class DataProviderServiceLocator : MonoBehaviour
    {
        static Dictionary<Type, IDataProvider> singletons = new Dictionary<Type, IDataProvider>();

        static Dictionary<Type, List<IDataProvider>> multipleSources =
            new Dictionary<Type, List<IDataProvider>>();

        public static void RegisterProvider<D>(DataProvider<D> data_provider)
        {
            var ty = typeof(D);

            if (singletons.TryGetValue(ty, out IDataProvider existing))
            {
                var _ = singletons.Remove(ty);
                multipleSources.Add(ty, new List<IDataProvider> { existing, data_provider });
            }
            else if (multipleSources.TryGetValue(ty, out List<IDataProvider> existing_multiple))
            {
                existing_multiple.Add(data_provider);
            }
            else
            {
                singletons.Add(ty, data_provider);
            }
        }

        public static void UnregisterProvider<D>(DataProvider<D> data_provider)
        {
            var ty = typeof(D);

            if (singletons.ContainsKey(ty))
            {
                var _ = singletons.Remove(ty);
            }
            else if (multipleSources.TryGetValue(ty, out List<IDataProvider> existing))
            {
                var _ = existing.Remove(data_provider);
            }
            else
            {
                Debug.Log($"Failed to unregister DataProvider<{ty}>: not registered");
            }
        }

        public static D FetchDataFromSingleton<D>()
        {
            var ty = typeof(D);

            if (singletons.TryGetValue(ty, out IDataProvider data_provider))
            {
                return (data_provider as DataProvider<D>).GetData();
            }
            else
            {
                Debug.LogError(
                    $"Failed to fetch DataProvider<{ty}> as a singleton: it's either not registered or have more than one instance"
                );
                throw new Exception();
            }
        }
    }
}
