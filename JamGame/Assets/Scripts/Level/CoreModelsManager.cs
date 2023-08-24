using Common;
using Level.Room;
using System.Collections.Generic;
using TileBuilder;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace Level
{
    [AddComponentMenu("Scripts/Level.CoreModelsManager")]
    public class CoreModelsManager : MonoBehaviour
    {
        [SerializeField]
        private AssetLabelReference coreModelsLabel;

        private Dictionary<string, IResourceLocation> hashPrefabsMap = new();

        private static CoreModelsManager instance = null;
        public static CoreModelsManager Instance => instance;

        private void Awake()
        {
            if (instance != null)
            {
                Debug.LogError("Singleton Instance Duplication.");
            }
            instance = this;
            foreach (
                AssetWithLocation<CoreModel> pair in AddressableTools<CoreModel>.LoadAllFromAssetLabel(
                    coreModelsLabel
                )
            )
            {
                hashPrefabsMap.Add(pair.Asset.HashCode, pair.Location);
            }
        }

        public CoreModel InstantiateCoreModel(TileConfig config)
        {
            CoreModel core = Instantiate(
                AddressableTools<CoreModel>.LoadAsset(hashPrefabsMap[config.HashCode]),
                transform
            );
            core.TileUnionModel.PlacingProperties.SetPositionAndRotation(
                config.Position,
                config.Rotation
            );
            return core;
        }

        public CoreModel InstantiateCoreModel(string hashCode)
        {
            return InstantiateCoreModel(new TileConfig(hashCode, Vector2Int.zero, 0));
        }
    }
}
