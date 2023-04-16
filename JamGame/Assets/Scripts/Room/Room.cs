using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InternalPathCollection))]
public class Room : MonoBehaviour
{
    [SerializeField] public RoomType roomType;
    [SerializeField] Dictionary<RoomType, Vector2Int> necessarilyNeighbours;
    [SerializeField] public Vector2Int currentDiraction = Vector2Int.up;

    public Vector2Int position;

    InternalPathCollection internalPaths;

    void Start()
    {
        internalPaths = transform.GetComponent<InternalPathCollection>();
    }

    public WallCollection GetWallVisualizer(Vector2Int diraction)
    {
        throw new NotImplementedException();
    }

    public RoomInternalPath GetInternalPath(Direction from, Direction to)
    {
        return internalPaths.GetPath(from, to);
    }
}