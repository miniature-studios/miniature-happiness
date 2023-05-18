using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TileWallPrefabHandler
{
    public TileWallType Type;
    public GameObject Prefab;
}
[Serializable]
public class TileCornerPrefabHandler
{
    public TileCornerType Type;
    public GameObject Prefab;
}

[CreateAssetMenu(fileName = "TileElementsHandler", menuName = "Builder/TileElementsHandler", order = 0)]
public class TileElementsHandler : ScriptableObject
{
    public List<TileWallPrefabHandler> WallPrefabHandlers;
    public List<TileCornerPrefabHandler> CornerPrefabHandlers;
}
