using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TilePrefabHandler
{
    public TileType Type;
    public GameObject Prefab;
}

[CreateAssetMenu(fileName = "TilePrefabsHandler", menuName = "ScriptableObjects/TilePrefabsHandler", order = 0)]
public class TilePrefabsHandler : ScriptableObject
{
    public List<TilePrefabHandler> TilePrefabHandlers;
}
