using Common;
using UnityEngine;

[CreateAssetMenu(fileName = "BuilderMatrix", menuName = "ScriptableObjects/BuilderMatrix", order = 3)]
public class BuilderMatrix : ScriptableObject
{
    [SerializeField] private float SelectingPlaneHeight = 1;
    [SerializeField] public int Step = 5;
    public Result<Vector2Int> GetMatrixPosition(Ray ray)
    {
        Plane plane = new(Vector3.up, new Vector3(0, SelectingPlaneHeight, 0));
        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            return new SuccessResult<Vector2Int>(new(Mathf.RoundToInt(-hitPoint.z / Step), Mathf.RoundToInt(hitPoint.x / Step)));
        }
        else
        {
            return new FailResult<Vector2Int>("No ray hits with matrix");
        }
    }
}

