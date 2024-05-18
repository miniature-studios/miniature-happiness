using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Common
{
    public static class AddressableTools
    {
        public static Dictionary<InternalUid, T> LoadAllGameObjectAssets<T>(
            AssetLabelReference assetLabel
        )
            where T : MonoBehaviour, IUidHandle
        {
            return LoadAllGameObjectAssets<T>((object)assetLabel);
        }

        public static Dictionary<InternalUid, T> LoadAllGameObjectAssets<T>(string assetLabel)
            where T : MonoBehaviour, IUidHandle
        {
            return LoadAllGameObjectAssets<T>((object)assetLabel);
        }

        private static Dictionary<InternalUid, T> LoadAllGameObjectAssets<T>(object assetLabel)
            where T : MonoBehaviour, IUidHandle
        {
            Dictionary<InternalUid, T> dictionary = new();
            IList<IResourceLocation> locations = Addressables
                .LoadResourceLocationsAsync(assetLabel)
                .WaitForCompletion();
            foreach (IResourceLocation resourceLocation in locations)
            {
                Result<T> result = TryLoadGameObjectAsset<T>(resourceLocation);
                if (result.Failure)
                {
                    Debug.LogError(result.Error);
                    continue;
                }

                InternalUid uid = result.Data.Uid;

                if (dictionary.ContainsKey(uid))
                {
                    Debug.LogError(
                        $"Uid duplication in {result.Data.gameObject.name} "
                            + $"and {dictionary[uid].gameObject.name} assets."
                    );
                    continue;
                }

                dictionary.Add(uid, result.Data);
            }
            return dictionary;
        }

        private static Result<T> TryLoadGameObjectAsset<T>(IResourceLocation resourceLocation)
            where T : MonoBehaviour, IUidHandle
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

        public static Dictionary<InternalUid, T> LoadAllScriptableObjectAssets<T>(
            AssetLabelReference assetLabel
        )
            where T : ScriptableObject, IUidHandle
        {
            return LoadAllScriptableObjectAssets<T>((object)assetLabel);
        }

        private static Dictionary<InternalUid, T> LoadAllScriptableObjectAssets<T>(
            object assetLabel
        )
            where T : ScriptableObject, IUidHandle
        {
            Dictionary<InternalUid, T> dictionary = new();
            IList<IResourceLocation> locations = Addressables
                .LoadResourceLocationsAsync(assetLabel)
                .WaitForCompletion();
            foreach (IResourceLocation resourceLocation in locations)
            {
                Result<T> result = TryLoadScriptableObjectAsset<T>(resourceLocation);
                if (result.Failure)
                {
                    Debug.LogError(result.Error);
                    continue;
                }

                InternalUid uid = result.Data.Uid;

                if (dictionary.ContainsKey(uid))
                {
                    Debug.LogError(
                        $"Uid duplication in {result.Data.name} "
                            + $"and {dictionary[uid].name} assets."
                    );
                    continue;
                }

                dictionary.Add(uid, result.Data);
            }
            return dictionary;
        }

        private static Result<T> TryLoadScriptableObjectAsset<T>(IResourceLocation resourceLocation)
            where T : ScriptableObject, IUidHandle
        {
            ScriptableObject scriptableObject = Addressables
                .LoadAssetAsync<ScriptableObject>(resourceLocation)
                .WaitForCompletion();
            if (scriptableObject is T casted)
            {
                return new SuccessResult<T>(casted);
            }
            else
            {
                return new FailResult<T>("Tried to load asset with wrong component.");
            }
        }
    }
}
