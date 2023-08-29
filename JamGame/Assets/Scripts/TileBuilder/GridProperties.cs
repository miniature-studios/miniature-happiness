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
                return new SuccessResult<Vector2Int>(
                    new(Mathf.RoundToInt(-hitPoint.z / Step), Mathf.RoundToInt(hitPoint.x / Step))
                );
            }
            else
            {
                return new FailResult<Vector2Int>("No ray hits with matrix");
            }
        }
    }
}
