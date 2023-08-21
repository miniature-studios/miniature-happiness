using Level.Room;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TileBuilder
{
    [Serializable]
    public struct TilePlaceConfig
    {
        public CoreModel CoreModel;
        public Vector2Int Position;
        public int Rotation;
    }

    [Serializable]
    [CreateAssetMenu(fileName = "BuildingConfig", menuName = "TileBuilder/BuildingConfig", order = 0)]
    public class BuildingConfig : ScriptableObject
    {
        public List<TilePlaceConfig> TilePlaceConfigs = new();
    }
}
