using System;
using System.Collections.Generic;
using UnityEngine;
using Common;

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

[CreateAssetMenu(fileName = "TileElementsHandler", menuName = "ScriptableObjects/TileElementsHandler", order = 2)]
public class TileElementsHandler : ScriptableObject
{
    public List<TileWallPrefabHandler> WallPrefabHandlers;
    public List<TileCornerPrefabHandler> CornerPrefabHandlers;
}
