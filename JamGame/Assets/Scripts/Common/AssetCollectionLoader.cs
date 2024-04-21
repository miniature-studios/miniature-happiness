using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Common
{
    [Serializable]
    internal class AssetCollectionLoader<T>
        where T : MonoBehaviour, IUidHandle
    {
        [Required]
        [SerializeField]
        private AssetLabelReference label;
        private Dictionary<InternalUid, IResourceLocation> uidResourceMap;
        public Dictionary<InternalUid, IResourceLocation> Collection => uidResourceMap;

        public void PrepareCollection()
        {
            uidResourceMap = new();
            IEnumerable<AssetWithLocation<T>> locations = AddressableTools<T>.LoadAllFromLabel(
                label
            );

            foreach (AssetWithLocation<T> asset in locations)
            {
                uidResourceMap.Add(asset.Asset.Uid, asset.Location);
            }
        }
    }
}
