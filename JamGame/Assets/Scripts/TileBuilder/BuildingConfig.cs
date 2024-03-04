using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Common;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TileBuilder
{
    [Serializable]
    public struct TileConfig
    {
        [ReadOnly]
        [SerializeField]
        private InternalUid uid;
        public InternalUid Uid => uid;

        [SerializeField]
        private Vector2Int position;
        public readonly Vector2Int Position => position;

        [SerializeField]
        private int rotation;
        public readonly int Rotation => rotation;

        public TileConfig(InternalUid uid, Vector2Int position, int rotation)
        {
            this.uid = uid;
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
