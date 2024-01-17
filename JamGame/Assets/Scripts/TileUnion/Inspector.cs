#if UNITY_EDITOR
using System.Linq;
using Common;
using Location;
using Sirenix.OdinInspector;
using TileUnion.Tile;
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
        public void ValidateProjectedTiles(int projectedCount)
        {
            if (IsAllWithMark("Outside"))
            {
                foreach (TileImpl tile in tiles)
                {
                    if (tile.ProjectedTiles.Count() != projectedCount && tile.TileToProject != null)
                    {
                        ProjectTile(tile, projectedCount);
                    }
                }
            }
        }

        [Button(Style = ButtonStyle.Box)]
        public void RecreateProjectedTiles(int projectedCount)
        {
            if (!IsAllWithMark("Outside"))
            {
                Debug.LogError("Projected Tiles valid only for outside tiles");
                return;
            }
            foreach (TileImpl tile in tiles)
            {
                if (tile.TileToProject == null)
                {
                    Debug.LogError($"Tile {tile.gameObject.name} have null TileToProject");
                    continue;
                }
                ProjectTile(tile, projectedCount);
            }
        }

        private void ProjectTile(TileImpl tile, int projectedCount)
        {
            Result result = tile.CreateProjectedTiles(gridProperties, projectedCount);
            if (result.Failure)
            {
                Debug.LogError($"Fail to recreate {tile.gameObject.name}: {result.Error}");
            }
        }
    }
}
#endif
