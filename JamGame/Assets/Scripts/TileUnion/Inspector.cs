#if UNITY_EDITOR
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
            foreach (TileImpl tile in Tiles)
            {
                tile.ShowDirectionGizmo = value;
            }
        }

        [Button(Style = ButtonStyle.Box)]
        public void SetPathGizmo(bool value)
        {
            foreach (TileImpl tile in Tiles)
            {
                tile.ShowPathGizmo = value;
            }
        }

        [Button(Style = ButtonStyle.Box)]
        public void SetCenterCube(bool value)
        {
            foreach (TileImpl tile in Tiles)
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
            foreach (TileImpl tile in Tiles)
            {
                tile.SetPosition(gridProperties, tile.Position);
                tile.SetRotation(tile.Rotation);
            }
        }
    }
}
#endif
