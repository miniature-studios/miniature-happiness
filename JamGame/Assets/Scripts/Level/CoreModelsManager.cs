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
        private AssetLabelReference coreModelsRef;

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
                LocationLinkPair<CoreModel> pair in AddressablesTools.LoadAllFromLabel<CoreModel>(
                    coreModelsRef
                )
            )
            {
                hashPrefabsMap.Add(pair.Link.HashCode, pair.ResourceLocation);
            }
        }

        public CoreModel InstantiateCoreModel(TileConfig config)
        {
            CoreModel core = Instantiate(
                AddressablesTools.LoadAsset<CoreModel>(hashPrefabsMap[config.HashCode]),
                transform
            );
            core.ConfigurateFromConfig(config);
            return core;
        }

        public CoreModel InstantiateCoreModel(string hashCode)
        {
            return InstantiateCoreModel(new TileConfig(hashCode, Vector2Int.zero, 0));
        }
    }
}
