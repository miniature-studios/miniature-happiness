using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomInternalPath
{
    public Direction from;
    public Direction to;

    // Store path curves here

    // methods:
    //  GetPath (for NPC movement)
    //  GetPathLength (maybe approx. for pathfinding)
}

public class Room : MonoBehaviour
{
    [SerializeField] RoomType roomType;
    [SerializeField] List<RoomInternalPath> internalPaths;

    public RoomInternalPath GetInternalPath(Direction from, Direction to)
    {
        throw new NotImplementedException();
    }
}
