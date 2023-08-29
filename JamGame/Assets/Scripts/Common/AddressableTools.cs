using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Common
{
    public struct AssetWithLocation<T>
        where T : class
    {
        public IResourceLocation Location;
        public T Asset;
    }

    public static class AddressableTools<T>
        where T : class
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
                yield return new AssetWithLocation<T>()
                {
                    Location = resourceLocation,
                    Asset = LoadAsset(resourceLocation)
                };
            }
        }

        public static T LoadAsset(IResourceLocation resourceLocation)
        {
            GameObject gameObject = Addressables
                .LoadAssetAsync<GameObject>(resourceLocation)
                .WaitForCompletion();
            return gameObject.GetComponent<T>();
        }
    }
}
