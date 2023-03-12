using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InternalPathCollection))]
public class Room : MonoBehaviour
{
    [SerializeField] public WallCollection up;
    [SerializeField] public WallCollection right;
    [SerializeField] public WallCollection down;
    [SerializeField] public WallCollection left;
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
        if (diraction == Vector2Int.up)
            return up;

        if (diraction == Vector2Int.right)
            return right;

        if (diraction == Vector2Int.down)
            return down;

        if (diraction == Vector2Int.left)
            return left;

        throw new ArgumentException();
    }

    public RoomInternalPath GetInternalPath(Direction from, Direction to)
    {
        return internalPaths.GetPath(from, to);
    }
}