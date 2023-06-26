using System.Collections.Generic;
using UnityEngine;

namespace TileUnion.Tile
{
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
