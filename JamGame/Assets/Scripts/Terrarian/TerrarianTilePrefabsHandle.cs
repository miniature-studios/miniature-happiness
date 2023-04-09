using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TerrarianTilePrefabMatch
{
    public TerrarianTileType terrarianTileType;
    public GameObject prefab;
}

[CreateAssetMenu(fileName = "TerrarianTilePrefabsHandle", menuName = "ScriptableObjects/TerrarianTilePrefabsHandle", order = 0)]
public class TerrarianTilePrefabsHandle : ScriptableObject
{
    public List<TerrarianTilePrefabMatch> terrarianTilePrefabMatches;
}
