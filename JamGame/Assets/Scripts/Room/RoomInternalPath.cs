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