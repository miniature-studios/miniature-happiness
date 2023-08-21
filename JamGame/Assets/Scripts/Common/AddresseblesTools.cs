using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Common
{
    public static class AddressableTools
    {
        public const string TileUnionsLabel = "TileUnion";
        public const string ShopViewsLabel = "ShopView";
        public const string InventoryViewsLabel = "InventoryView";
        public const string CoreModelsLabel = "CoreModel";

        public static IEnumerable<GameObject> GetAllAssetsByLabel(string label)
        {
            AsyncOperationHandle<IList<GameObject>> operation =
                Addressables.LoadAssetsAsync<GameObject>(label, null);
            return operation.WaitForCompletion();
        }
    }
}
