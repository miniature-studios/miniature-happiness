using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WallPlacementRule
{
    public RoomType room_a;
    public RoomType room_b;
    public WallType wall;
}
public class DiractlyAllowedWalls
{
    public RoomType room;
    public Vector2Int diraction;
    public List<WallType> walls;
}

[CreateAssetMenu(fileName = "WallPlacementRules", menuName = "ScriptableObjects/WallPlacementRules", order = 1)]
public class WallPlacementRules : ScriptableObject
{
    public List<WallPlacementRule> neighborsLimitation;
    public List<DiractlyAllowedWalls> diractlyAllowedWalls;
}
