using Unity.AI.Navigation;
using UnityEngine;

[RequireComponent(typeof(NavMeshSurface))]
public class NavMeshSurfaceUpdater : MonoBehaviour
{
    private NavMeshSurface surface;

    private void Start()
    {
        surface = GetComponent<NavMeshSurface>();
        surface.BuildNavMesh();
    }

    public void UpdateNavMesh()
    {
        _ = surface.UpdateNavMesh(surface.navMeshData);
    }
}
