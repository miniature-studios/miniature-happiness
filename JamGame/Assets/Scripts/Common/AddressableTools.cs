using System;
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
                Result<T> result = TryLoadAsset(resourceLocation);
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

        public static T LoadAsset(IResourceLocation resourceLocation)
        {
            Result<T> result = TryLoadAsset(resourceLocation);
            if (result.Failure)
            {
                throw new Exception(result.Error);
            }
            return result.Data;
        }
    }

    public static class AddressableTools
    {
        public static Dictionary<InternalUid, IResourceLocation> LoadResourceLocationsByUid<T>(
            AssetLabelReference label
        )
            where T : MonoBehaviour, IUidHandle
        {
            Dictionary<InternalUid, IResourceLocation> uidResourceMap = new();
            IEnumerable<AssetWithLocation<T>> locations = AddressableTools<T>.LoadAllFromLabel(
                label
            );

            foreach (AssetWithLocation<T> asset in locations)
            {
                uidResourceMap.Add(asset.Asset.Uid, asset.Location);
            }

            return uidResourceMap;
        }
    }
}
