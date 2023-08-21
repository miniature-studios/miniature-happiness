using Level.Room;
using Pickle;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace TileBuilder
{
    [Serializable]
    public struct TilePlaceConfig
    {
        [SerializeField]
        [Pickle(LookupType = ObjectProviderType.Assets)]
        private CoreModel coreModel;
        public readonly CoreModel CoreModel => coreModel;

        [SerializeField]
        private Vector2Int position;
        public readonly Vector2Int Position => position;

        [SerializeField]
        private int rotation;
        public readonly int Rotation => rotation;

        public TilePlaceConfig(CoreModel coreModel, Vector2Int position, int rotation)
        {
            this.coreModel = coreModel;
            this.position = position;
            this.rotation = rotation;
        }
    }

    [Serializable]
    [CreateAssetMenu(
        fileName = "BuildingConfig",
        menuName = "TileBuilder/BuildingConfig",
        order = 0
    )]
    public class BuildingConfig : ScriptableObject
    {
        [SerializeField]
        private List<TilePlaceConfig> tilePlaceConfigs = new();
        public ImmutableList<TilePlaceConfig> TilePlaceConfigs =>
            tilePlaceConfigs.ToImmutableList();

        public void Init(List<TilePlaceConfig> tilePlaceConfigs)
        {
            this.tilePlaceConfigs = tilePlaceConfigs;
        }

        public static BuildingConfig CreateInstance(List<TilePlaceConfig> tilePlaceConfigs)
        {
            BuildingConfig data = CreateInstance<BuildingConfig>();
            data.Init(tilePlaceConfigs);
            return data;
        }
    }
}
