using Common;
using System.Collections.Generic;
using UnityEngine;

public class InternalPathCollection : MonoBehaviour
{
    // DEBUG - public for InternalPathCollectionEditor
    [SerializeField] public List<RoomInternalPath> paths = new();

    public RoomInternalPath GetPath(Direction from, Direction to)
    {
        return paths.Find(r =>
            r.from == from && r.to == to
        );
    }

    private void OnDrawGizmos()
    {
        foreach (RoomInternalPath internalPath in paths)
        {
            if (internalPath.linePoints.Length < 4)
            {
                continue;
            }

            Vector3 preveousePoint = internalPath.linePoints[0];

            for (int i = 0; i < RoomInternalPath.sigmentsNumber + 1; i++)
            {
                float paremeter = (float)i / RoomInternalPath.sigmentsNumber;
                Vector3 point = Bezier.GetPoint(internalPath.linePoints[0], internalPath.linePoints[1], internalPath.linePoints[2], internalPath.linePoints[3], paremeter);
                Gizmos.DrawLine(preveousePoint, point);
                preveousePoint = point;
            }
            foreach (Vector3 item in internalPath.linePoints)
            {
                Gizmos.DrawSphere(item, 0.2f);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        foreach (RoomInternalPath internalPath in paths)
        {
            foreach (Vector3 item in internalPath.linePoints)
            {
                Gizmos.DrawWireSphere(item, 0.21f);
            }
        }
    }
}
