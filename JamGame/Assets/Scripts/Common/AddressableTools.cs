using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Common
{
    public struct AssetWithLocation<T>
        where T : MonoBehaviour
    {
        public IResourceLocation Location;
        public T Asset;
    }

    public static class AddressableTools<T>
        where T : MonoBehaviour, IUidHandle
    {
        public static Dictionary<InternalUid, T> LoadAllFromLabel(AssetLabelReference assetLabel)
        {
            return LoadAllFromLabel((object)assetLabel);
        }

        public static Dictionary<InternalUid, T> LoadAllFromLabel(string assetLabel)
        {
            return LoadAllFromLabel((object)assetLabel);
        }

        private static Dictionary<InternalUid, T> LoadAllFromLabel(object assetLabel)
        {
            Dictionary<InternalUid, T> dictionary = new();
            IList<IResourceLocation> locations = Addressables
                .LoadResourceLocationsAsync(assetLabel)
                .WaitForCompletion();
            foreach (IResourceLocation resourceLocation in locations)
            {
                Result<T> result = TryLoadAsset(resourceLocation);
                if (result.Success && result.Data.TryGetComponent(out T _))
                {
                    dictionary.Add(result.Data.Uid, result.Data);
                }
            }
            return dictionary;
        }

        private static Result<T> TryLoadAsset(IResourceLocation resourceLocation)
        {
            GameObject gameObject = Addressables
                .LoadAssetAsync<GameObject>(resourceLocation)
                .WaitForCompletion();
            if (gameObject.TryGetComponent(out T component))
            {
                return new SuccessResult<T>(component);
            }
            else
            {
                return new FailResult<T>("Tried to load asset with wrong component.");
            }
        }
    }
}
