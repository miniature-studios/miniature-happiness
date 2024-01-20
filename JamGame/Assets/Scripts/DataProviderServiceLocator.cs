using System;
using System.Collections.Generic;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

public class DataProviderServiceLocator : MonoBehaviour
{
    [SerializeReference]
    [ReadOnly]
    private Dictionary<Type, IDataProvider> singletons = new();

    [SerializeReference]
    [ReadOnly]
    private Dictionary<Type, List<IDataProvider>> multipleSources = new();

    private static DataProviderServiceLocator instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError(
                "Two or more instances of DataProviderServiceLocator are added to scene"
            );
            return;
        }

        instance = this;
    }

    public static void RegisterProvider<D>(DataProvider<D> data_provider)
    {
        instance.RegisterProviderInstance(data_provider);
    }

    private void RegisterProviderInstance<D>(DataProvider<D> data_provider)
    {
        Type type = typeof(D);

        if (singletons.TryGetValue(type, out IDataProvider existing))
        {
            _ = singletons.Remove(type);
            multipleSources.Add(type, new List<IDataProvider> { existing, data_provider });
        }
        else if (multipleSources.TryGetValue(type, out List<IDataProvider> existing_multiple))
        {
            existing_multiple.Add(data_provider);
        }
        else
        {
            singletons.Add(type, data_provider);
        }
    }

    public static void UnregisterProvider<D>(DataProvider<D> data_provider)
    {
        instance.UnregisterProviderInstance(data_provider);
    }

    private void UnregisterProviderInstance<D>(DataProvider<D> data_provider)
    {
        Type type = typeof(D);

        if (singletons.ContainsKey(type))
        {
            _ = singletons.Remove(type);
        }
        else if (multipleSources.TryGetValue(type, out List<IDataProvider> existing))
        {
            _ = existing.Remove(data_provider);
        }
        else
        {
            Debug.LogError($"Failed to unregister DataProvider<{type}>: not registered");
        }
    }

    public static D FetchDataFromSingleton<D>()
    {
        return instance.FetchDataFromSingletonInstance<D>();
    }

    public D FetchDataFromSingletonInstance<D>()
    {
        Type type = typeof(D);

        if (singletons.TryGetValue(type, out IDataProvider data_provider))
        {
            return (data_provider as DataProvider<D>).GetData();
        }
        else
        {
            Debug.LogError(
                $"Failed to fetch DataProvider<{type}> as a singleton: it's either not registered or have more than one instance"
            );
            throw new Exception();
        }
    }
}
