using Common;
using System.Collections.Generic;
using TileBuilder;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Level.Room
{
    public static class CoreModelTools
    {
        private static string coreModelsLabel = "CoreModel";

        private static Dictionary<string, IResourceLocation> hashPrefabsMap = new();

        static CoreModelTools()
        {
            foreach (
                AssetWithLocation<CoreModel> core in AddressableTools<CoreModel>.LoadAllFromStringLabel(
                    coreModelsLabel
                )
            )
            {
                hashPrefabsMap.Add(core.Asset.HashCode, core.Location);
            }
        }

        public static CoreModel InstantiateCoreModel(TileConfig config)
        {
            CoreModel core = GameObject.Instantiate(
                AddressableTools<CoreModel>.LoadAsset(hashPrefabsMap[config.HashCode])
            );
            core.TileUnionModel.PlacingProperties.SetPositionAndRotation(
                config.Position,
                config.Rotation
            );
            return core;
        }

        public static CoreModel InstantiateCoreModel(string hashCode)
        {
            return InstantiateCoreModel(new TileConfig(hashCode, Vector2Int.zero, 0));
        }
    }
}
