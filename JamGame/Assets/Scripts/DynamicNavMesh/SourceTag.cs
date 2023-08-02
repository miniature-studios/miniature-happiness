using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace DynamicNavMesh
{
    // Tagging component for use with the LocalNavMeshBuilder
    // Supports mesh-filter - can be extended to physics and/or primitives
    [DefaultExecutionOrder(-200)]
    [AddComponentMenu("DynamicNavMesh.SourceTag")]
    public class SourceTag : MonoBehaviour
    {
        // Global containers for all active mesh tags
        public static List<MeshFilter> Meshes = new();

        private void OnEnable()
        {
            if (TryGetComponent(out MeshFilter mf))
            {
                Meshes.Add(mf);
            }
        }

        private void OnDisable()
        {
            if (TryGetComponent(out MeshFilter mf))
            {
                _ = Meshes.Remove(mf);
            }
        }

        // Collect all the navmesh build sources for enabled objects tagged by this component
        public static void Collect(ref List<NavMeshBuildSource> sources)
        {
            sources.Clear();

            for (int i = 0; i < Meshes.Count; ++i)
            {
                MeshFilter mf = Meshes[i];
                if (mf == null)
                {
                    continue;
                }

                Mesh m = mf.sharedMesh;
                if (m == null)
                {
                    continue;
                }

                NavMeshBuildSource s =
                    new()
                    {
                        shape = NavMeshBuildSourceShape.Mesh,
                        sourceObject = m,
                        transform = mf.transform.localToWorldMatrix,
                        area = 0
                    };
                sources.Add(s);
            }
        }
    }
}
