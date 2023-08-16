using Unity.AI.Navigation;
using UnityEngine;

namespace Scripts
{
    // TODO: apply optimization
    [RequireComponent(typeof(NavMeshSurface))]
    public class NavMeshSurfaceUpdater : MonoBehaviour
    {
        private NavMeshSurface surface;

        private void Start()
        {
            surface = GetComponent<NavMeshSurface>();
            surface.BuildNavMesh();
        }

        private void Update()
        {
            _ = surface.UpdateNavMesh(surface.navMeshData);
        }
    }
}
