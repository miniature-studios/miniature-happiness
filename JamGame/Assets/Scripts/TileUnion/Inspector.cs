#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Location;
using Sirenix.OdinInspector;
using TileUnion.Tile;
using UnityEditor;
using UnityEngine;

namespace TileUnion
{
    public partial class TileUnionImpl
    {
        [Button(Style = ButtonStyle.Box)]
        public void SetDirectionsGizmo(bool value)
        {
            foreach (TileImpl tile in tiles)
            {
                tile.ShowDirectionGizmo = value;
            }
        }

        [Button(Style = ButtonStyle.Box)]
        public void SetPathGizmo(bool value)
        {
            foreach (TileImpl tile in tiles)
            {
                tile.ShowPathGizmo = value;
            }
        }

        [Button(Style = ButtonStyle.Box)]
        public void SetCenterCube(bool value)
        {
            foreach (TileImpl tile in tiles)
            {
                tile.SetCubeInCenter(value);
            }
        }

        [Button(Style = ButtonStyle.Box)]
        public void SetDrawNeedProviderGizmo(bool value)
        {
            DrawNeedProviderGizmo = value;
        }

        [HideInInspector]
        public bool DrawNeedProviderGizmo = false;

        private void OnDrawGizmos()
        {
            if (DrawNeedProviderGizmo)
            {
                DrawGizmoRecursively(transform);
            }
        }

        private void DrawGizmoRecursively(Transform transform)
        {
            foreach (Transform child in transform)
            {
                if (child.TryGetComponent<NeedProvider>(out _))
                {
                    Gizmos.color = Color.blue;
                    Gizmos.DrawSphere(child.position, 0.2f);
                    Gizmos.color = Color.white;
                    Gizmos.DrawSphere(child.position, 0.1f);
                }
                DrawGizmoRecursively(child);
            }
        }

        [Button("Update tiles position and rotation", Style = ButtonStyle.Box)]
        public void UpdateTilesPositionAndRotation()
        {
            foreach (TileImpl tile in tiles)
            {
                tile.SetPosition(gridProperties, tile.Position);
                tile.SetRotation(tile.Rotation);
            }
        }

        [Button(Style = ButtonStyle.Box)]
        private void CreateProjectedTiles()
        {
            if (!IsAllWithMark("Outside"))
            {
                Debug.LogError("Projected Tiles valid only for outside tiles");
                return;
            }
            if (projectedTilesRoot == null)
            {
                GameObject gameObject = new("Projected Root");
                projectedTilesRoot = Instantiate(gameObject, transform).transform;
            }
            while (projectedTilesRoot.childCount > 0)
            {
                DestroyImmediate(projectedTilesRoot.GetChild(0).gameObject);
            }
            projectedTiles.Clear();
            foreach (TileImpl tile in tiles)
            {
                projectedTiles.Add(tile, new List<TileImpl>());
            }
            foreach (KeyValuePair<TileImpl, List<TileImpl>> pair in projectedTiles)
            {
                pair.Value.Clear();
                for (int i = 0; i < projectedTilesCount; i++)
                {
                    Object gameObject = PrefabUtility.InstantiatePrefab(
                        tileToProject,
                        projectedTilesRoot
                    );
                    TileImpl instance = (gameObject as GameObject).GetComponent<TileImpl>();

                    instance.transform.SetPositionAndRotation(
                        pair.Key.transform.position
                            + ((pair.Value.Count() + 1) * gridProperties.TileHeight * Vector3.down),
                        pair.Key.transform.rotation
                    );
                    pair.Value.Add(instance);
                }
            }
        }
    }
}
#endif
