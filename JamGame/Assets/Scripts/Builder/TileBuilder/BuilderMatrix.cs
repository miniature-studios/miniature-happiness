using Common;
using UnityEngine;

[CreateAssetMenu(fileName = "BuilderMatrix", menuName = "Builder/BuilderMatrix", order = 2)]
public class BuilderMatrix : ScriptableObject
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
            Vector3 hit_point = ray.GetPoint(enter);
            return new SuccessResult<Vector2Int>(
                new(Mathf.RoundToInt(-hit_point.z / Step), Mathf.RoundToInt(hit_point.x / Step))
            );
        }
        else
        {
            return new FailResult<Vector2Int>("No ray hits with matrix");
        }
    }
}
