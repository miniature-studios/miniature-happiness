using System.Collections.Generic;
using System.Linq;
using TileBuilder;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace SettingsConfigs
{
    public static class GlobalGameSettings
    {
        private static string gameSettingsLabel = "GameSettings";
        private static string projectedTilesSettingsLabel = "ProjectedTilesSettings";

        public static GridProperties GetGridProperties()
        {
            return LoadScriptableObjectFromLabel<GameSettings>(gameSettingsLabel).Matrix;
        }

        public static ProjectedTilesSettings GetProjectedTilesSettings()
        {
            return LoadScriptableObjectFromLabel<ProjectedTilesSettings>(
                projectedTilesSettingsLabel
            );
        }

        public static void SetGridProperties(GridProperties gridProperties)
        {
            LoadScriptableObjectFromLabel<GameSettings>(gameSettingsLabel)
                .SetMatrix(gridProperties);
        }

        private static T LoadScriptableObjectFromLabel<T>(string assetLabel)
            where T : ScriptableObject
        {
            IList<IResourceLocation> list = Addressables
                .LoadResourceLocationsAsync(assetLabel, typeof(ScriptableObject))
                .WaitForCompletion();
            return Addressables.LoadAssetAsync<ScriptableObject>(list.First()).WaitForCompletion()
                as T;
        }
    }
}
