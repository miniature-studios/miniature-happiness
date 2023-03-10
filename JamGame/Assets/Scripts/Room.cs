using Common;
using System;
using UnityEngine;

public class RoomInternalPath
{
    // methoods:
    //  GetPath (for NPC movement)
    //  GetPathLength (maybe approx. for pathfinding)
}

public class Room : MonoBehaviour
{
    public RoomInternalPath GetInternalPath(Direction from, Direction to)
    {
        throw new NotImplementedException();
    }
}
