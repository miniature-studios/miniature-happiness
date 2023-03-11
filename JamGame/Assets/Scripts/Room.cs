using Common;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomInternalPath
{
    public static int sigmentsNumber = 25;
    public Direction from;
    public Direction to;

    public Vector3[] linePoints = new Vector3[4];
    public Vector3 GetPathPoint(float normalizedTime)
    {
        return Bezier.GetPoint(linePoints[0], linePoints[1], linePoints[2], linePoints[3], normalizedTime);
    }
    public float GetPathLength()
    {
        float sum = 0;
        List<Vector3> path = new List<Vector3>();
        path.Add(linePoints[0]);
        for (int i = 0; i < sigmentsNumber + 1; i++)
        {
            float paremeter = (float)i / sigmentsNumber;
            Vector3 point = Bezier.GetPoint(linePoints[0], linePoints[1], linePoints[2], linePoints[3], paremeter);
            path.Add(point);
        }
        for (int i = 0; i < path.Count - 1; i++)
        {
            sum += Vector3.Distance(path[i], path[i + 1]);
        }
        return sum;
    }
}

public class Room : MonoBehaviour
{
    [SerializeField] public RoomType roomType;
    [SerializeField] Dictionary<RoomType, Vector2Int> necessarilyNeighbours;
    [SerializeField] public Vector2Int currentDiraction = Vector2Int.up;
    [SerializeField] List<RoomInternalPath> internalPaths;
    public RoomInternalPath GetInternalPath(Direction from, Direction to)
    {
        return internalPaths.Find(r => r.from == from && r.to == to);
    }
    private void OnDrawGizmos()
    {
        foreach (var internalPath in internalPaths)
        {
            Vector3 preveousePoint = internalPath.linePoints[0];

            for (int i = 0; i < RoomInternalPath.sigmentsNumber + 1; i++)
            {
                float paremeter = (float)i / RoomInternalPath.sigmentsNumber;
                Vector3 point = Bezier.GetPoint(internalPath.linePoints[0], internalPath.linePoints[1], internalPath.linePoints[2], internalPath.linePoints[3], paremeter);
                Gizmos.DrawLine(preveousePoint, point);
                preveousePoint = point;
            }
            foreach (var item in internalPath.linePoints)
            {
                Gizmos.DrawSphere(item, 0.2f);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        foreach (var internalPath in internalPaths)
        {
            foreach (var item in internalPath.linePoints)
            {
                Gizmos.DrawWireSphere(item, 0.21f);
            }
        }
    }
}