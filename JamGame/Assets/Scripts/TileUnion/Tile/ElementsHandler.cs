using System;
using System.Collections.Generic;
using UnityEngine;

namespace TileUnion.Tile
{
    [Serializable]
    public class WallPrefabHandler
    {
        public WallType Type;
        public GameObject Prefab;
    }

    [Serializable]
    public class CornerPrefabHandler
    {
        public CornerType Type;
        public GameObject Prefab;
    }

    // TODO: Move to the file to Tile?.
    [CreateAssetMenu(
        fileName = "TileElementsHandler",
        menuName = "Builder/TileElementsHandler",
        order = 0
    )]
    public class ElementsHandler : ScriptableObject
    {
        public List<WallPrefabHandler> WallPrefabHandlers;
        public List<CornerPrefabHandler> CornerPrefabHandlers;
    }
}
