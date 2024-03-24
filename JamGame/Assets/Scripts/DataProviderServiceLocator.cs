using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

public class DataProviderServiceLocator : MonoBehaviour
{
    public enum ResolveType
    {
        Singleton,
        MultipleSources
    }

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

    public static void Register<D>(DataProvider<D> data_provider, ResolveType resolve_type)
    {
        switch (resolve_type)
        {
            case ResolveType.Singleton:
                instance.RegisterSingleton(data_provider);
                break;
            case ResolveType.MultipleSources:
                instance.RegisterMultipleSources(data_provider);
                break;
            default:
                throw new ArgumentException("Unknown ResolveType variant");
        }
    }

    private void RegisterMultipleSources<D>(DataProvider<D> data_provider)
    {
        Type type = typeof(D);

        if (singletons.ContainsKey(type))
        {
            Debug.LogError("Data provider type is already registered as singleton.");
            return;
        }

        if (multipleSources.TryGetValue(type, out List<IDataProvider> existing_multiple))
        {
            existing_multiple.Add(data_provider);
        }
        else
        {
            multipleSources.Add(type, new List<IDataProvider> { data_provider });
        }
    }

    private void RegisterSingleton<D>(DataProvider<D> data_provider)
    {
        Type type = typeof(D);

        if (multipleSources.ContainsKey(type))
        {
            Debug.LogError("Data provider type is already registered as multiple source.");
            return;
        }

        if (singletons.ContainsKey(type))
        {
            Debug.LogError("Tried to register singleton data provider single time.");
            return;
        }

        singletons.Add(type, data_provider);
    }

    public static void Unregister<D>(DataProvider<D> data_provider)
    {
        instance.InstanceUnregister(data_provider);
    }

    private void InstanceUnregister<D>(DataProvider<D> data_provider)
    {
        Type type = typeof(D);

        if (singletons.ContainsKey(type))
        {
            _ = singletons.Remove(type);
        }
        else if (multipleSources.TryGetValue(type, out List<IDataProvider> existing))
        {
            _ = existing.Remove(data_provider);
            if (existing.Count == 0)
            {
                _ = multipleSources.Remove(type);
            }
        }
        else
        {
            Debug.LogError($"Failed to unregister DataProvider<{type}>: not registered");
        }
    }

    public static D FetchDataFromSingleton<D>()
    {
        Type type = typeof(D);

        if (instance.singletons.TryGetValue(type, out IDataProvider data_provider))
        {
            return (data_provider as DataProvider<D>).GetData();
        }
        else
        {
            Debug.LogError(
                $"Failed to fetch DataProvider<{type}> as a singleton: it's either not registered or registered as multiple source"
            );
            throw new Exception();
        }
    }

    public static IEnumerable<D> FetchDataFromMultipleSources<D>()
    {
        Type type = typeof(D);

        if (instance.singletons.ContainsKey(type))
        {
            Debug.LogError(
                $"Failed to fetch DataProvider<{type}> as a multiple sources: it's registered as singleton"
            );
            throw new Exception();
        }

        if (instance.multipleSources.TryGetValue(type, out List<IDataProvider> data_providers))
        {
            return data_providers.Select(p => (p as DataProvider<D>).GetData());
        }
        else
        {
            return new List<D>();
        }
    }
}
