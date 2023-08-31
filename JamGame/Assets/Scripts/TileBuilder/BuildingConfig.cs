using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace TileBuilder
{
    [Serializable]
    public struct TileConfig
    {
        [SerializeField]
        private string hashCode;
        public string HashCode => hashCode;

        [SerializeField]
        private Vector2Int position;
        public readonly Vector2Int Position => position;

        [SerializeField]
        private int rotation;
        public readonly int Rotation => rotation;

        public TileConfig(string hashCode, Vector2Int position, int rotation)
        {
            this.hashCode = hashCode;
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
        private List<TileConfig> tilePlaceConfigs = new();
        public ImmutableList<TileConfig> TilePlaceConfigs => tilePlaceConfigs.ToImmutableList();

        public void Init(List<TileConfig> tilePlaceConfigs)
        {
            this.tilePlaceConfigs = tilePlaceConfigs;
        }

        public static BuildingConfig CreateInstance(List<TileConfig> tilePlaceConfigs)
        {
            BuildingConfig data = CreateInstance<BuildingConfig>();
            data.Init(tilePlaceConfigs);
            return data;
        }
    }
}
