﻿using System.Collections.Generic;
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
        public static IEnumerable<AssetWithLocation<T>> LoadAllFromAssetLabel(
            AssetLabelReference assetLabel
        )
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

        public static IEnumerable<AssetWithLocation<T>> LoadAllFromStringLabel(string assetLabel)
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