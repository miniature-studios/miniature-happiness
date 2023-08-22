using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Common
{
    public struct LocationLinkPair<T>
    {
        public IResourceLocation ResourceLocation;
        public T Link;
    }

    public static class AddressablesTools
    {
        public static IEnumerable<LocationLinkPair<T>> LoadAllFromLabel<T>(
            AssetLabelReference assetLabel
        )
        {
            foreach (
                IResourceLocation resourceLocation in Addressables
                    .LoadResourceLocationsAsync(assetLabel, typeof(GameObject))
                    .WaitForCompletion()
            )
            {
                yield return new LocationLinkPair<T>()
                {
                    ResourceLocation = resourceLocation,
                    Link = LoadAsset<T>(resourceLocation)
                };
            }
        }

        public static IEnumerable<LocationLinkPair<T>> LoadAllFromLabel<T>(string assetLabel)
        {
            foreach (
                IResourceLocation resourceLocation in Addressables
                    .LoadResourceLocationsAsync(assetLabel, typeof(GameObject))
                    .WaitForCompletion()
            )
            {
                yield return new LocationLinkPair<T>()
                {
                    ResourceLocation = resourceLocation,
                    Link = LoadAsset<T>(resourceLocation)
                };
            }
        }

        public static T LoadAsset<T>(IResourceLocation resourceLocation)
        {
            GameObject gameObject = Addressables
                .LoadAssetAsync<GameObject>(resourceLocation)
                .WaitForCompletion();
            return gameObject.GetComponent<T>();
        }
    }
}
