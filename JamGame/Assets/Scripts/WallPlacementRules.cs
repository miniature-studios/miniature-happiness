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

[CreateAssetMenu(fileName = "WallPlacementRules", menuName = "ScriptableObjects/WallPlacementRules", order = 1)]
public class WallPlacementRules : ScriptableObject
{
    public List<WallPlacementRule> rules;
}
