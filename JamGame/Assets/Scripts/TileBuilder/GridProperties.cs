using Common;
using UnityEngine;

namespace TileBuilder
{
    [CreateAssetMenu(
        fileName = "GridProperties",
        menuName = "TileBuilder/GridProperties",
        order = 2
    )]
    public class GridProperties : ScriptableObject
    {
        [SerializeField]
        private float selectingPlaneHeight;

        [SerializeField]
        private int step;

        public int Step => step;

        public Result<Vector2Int> GetMatrixPosition(Ray ray)
        {
            Plane plane = new(Vector3.up, new Vector3(0, selectingPlaneHeight, 0));
            if (plane.Raycast(ray, out float enter))
            {
                Vector3 hitPoint = ray.GetPoint(enter);
                return new SuccessResult<Vector2Int>(GetMatrixPosition(hitPoint));
            }
            else
            {
                return new FailResult<Vector2Int>("No ray hits with matrix");
            }
        }

        public Vector2Int GetMatrixPosition(Vector3 point)
        {
            return new(Mathf.RoundToInt(-point.z / Step), Mathf.RoundToInt(point.x / Step));
        }

        public Vector3 GetWorldPoint(Vector2Int point)
        {
            return new(point.y * Step, 0, -point.x * Step);
        }
    }
}
