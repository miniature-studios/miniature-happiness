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
        where T : MonoBehaviour
    {
        public static IEnumerable<AssetWithLocation<T>> LoadAllFromLabel(
            AssetLabelReference assetLabel
        )
        {
            return LoadAllFromLabel((object)assetLabel);
        }

        public static IEnumerable<AssetWithLocation<T>> LoadAllFromLabel(string assetLabel)
        {
            return LoadAllFromLabel((object)assetLabel);
        }

        private static IEnumerable<AssetWithLocation<T>> LoadAllFromLabel(object assetLabel)
        {
            foreach (
                IResourceLocation resourceLocation in Addressables
                    .LoadResourceLocationsAsync(assetLabel, typeof(GameObject))
                    .WaitForCompletion()
            )
            {
                Result<T> result = LoadAsset(resourceLocation);
                if (result.Success)
                {
                    yield return new AssetWithLocation<T>()
                    {
                        Location = resourceLocation,
                        Asset = result.Data
                    };
                }
            }
        }

        public static Result<T> LoadAsset(IResourceLocation resourceLocation)
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
                return new FailResult<T>("Try to load asset with wrong component.");
            }
        }
    }
}
