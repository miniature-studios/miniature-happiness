using System.Collections.Generic;
using System.Linq;
using TileBuilder;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Utils
{
    public static class GlobalGameSettings
    {
        private static string gameSettingsLabel = "GameSettings";

        public static GridProperties GetGridProperties()
        {
            return LoadScriptableObjectFromLabel(gameSettingsLabel).Matrix;
        }

        public static void SetGridProperties(GridProperties matrix)
        {
            LoadScriptableObjectFromLabel(gameSettingsLabel).SetMatrix(matrix);
        }

        private static GameSettings LoadScriptableObjectFromLabel(string assetLabel)
        {
            IList<IResourceLocation> list = Addressables
                .LoadResourceLocationsAsync(assetLabel, typeof(ScriptableObject))
                .WaitForCompletion();
            return Addressables.LoadAssetAsync<ScriptableObject>(list.First()).WaitForCompletion() as GameSettings;
        }
    }
}
