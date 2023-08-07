using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NavMeshBuilder = UnityEngine.AI.NavMeshBuilder;

namespace DynamicNavMesh
{
    // Build and update a localized nav mesh from the sources marked by NavMeshSourceTag
    [DefaultExecutionOrder(-102)]
    [AddComponentMenu("Scripts/DynamicNavMesh.Builder")]
    public class Builder : MonoBehaviour
    {
        // The center of the build
        public Transform Tracked;

        // The size of the build bounds
        public Vector3 Size = new(80.0f, 20.0f, 80.0f);

        [SerializeField, InspectorReadOnly]
        private NavMeshData navMesh;

        [SerializeField, InspectorReadOnly]
        private AsyncOperation operation;

        [SerializeField, InspectorReadOnly]
        private NavMeshDataInstance instance;

        [SerializeField, InspectorReadOnly]
        private List<NavMeshBuildSource> sources = new();

        private IEnumerator Start()
        {
            while (true)
            {
                UpdateNavMesh(true);
                yield return operation;
            }
        }

        private void OnEnable()
        {
            // Construct and add nav mesh
            navMesh = new NavMeshData();
            instance = NavMesh.AddNavMeshData(navMesh);
            if (Tracked == null)
            {
                Tracked = transform;
            }

            UpdateNavMesh(false);
        }

        private void OnDisable()
        {
            // Unload nav mesh and clear handle
            instance.Remove();
        }

        private void UpdateNavMesh(bool asyncUpdate = false)
        {
            SourceTag.Collect(ref sources);
            NavMeshBuildSettings defaultBuildSettings = NavMesh.GetSettingsByID(0);
            Bounds bounds = QuantizedBounds();

            if (asyncUpdate)
            {
                operation = NavMeshBuilder.UpdateNavMeshDataAsync(
                    navMesh,
                    defaultBuildSettings,
                    sources,
                    bounds
                );
            }
            else
            {
                _ = NavMeshBuilder.UpdateNavMeshData(
                    navMesh,
                    defaultBuildSettings,
                    sources,
                    bounds
                );
            }
        }

        private static Vector3 Quantize(Vector3 v, Vector3 quant)
        {
            float x = quant.x * Mathf.Floor(v.x / quant.x);
            float y = quant.y * Mathf.Floor(v.y / quant.y);
            float z = quant.z * Mathf.Floor(v.z / quant.z);
            return new Vector3(x, y, z);
        }

        private Bounds QuantizedBounds()
        {
            // Quantize the bounds to update only when there is a 10% change in size
            Vector3 center = Tracked ? Tracked.position : transform.position;
            return new Bounds(Quantize(center, 0.1f * Size), Size);
        }

        private void OnDrawGizmosSelected()
        {
            if (navMesh)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(navMesh.sourceBounds.center, navMesh.sourceBounds.size);
            }

            Gizmos.color = Color.yellow;
            Bounds bounds = QuantizedBounds();
            Gizmos.DrawWireCube(bounds.center, bounds.size);

            Gizmos.color = Color.green;
            Vector3 center = Tracked ? Tracked.position : transform.position;
            Gizmos.DrawWireCube(center, Size);
        }
    }
}
