#if UNITY_EDITOR
using Common;
using Sirenix.OdinInspector;
using TileBuilder;
using TileUnion;
using UnityEngine;

namespace BuildingEditor
{
    [AddComponentMenu("Scripts/BuildingEditor/BuildingEditor.GizmoChanger")]
    internal class GizmoChanger : MonoBehaviour
    {
        [Required]
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        [ReadOnly]
        [SerializeField]
        private bool showPathGizmo = false;

        [ReadOnly]
        [SerializeField]
        private bool showHiddenTilesCube = false;

        private void Awake()
        {
            tileBuilder.OnTileUnionCreated += OnTileUnionCreated;
            OnShowHiddenTilesChanged(true);
        }

        private void OnTileUnionCreated(TileUnionImpl tileUnion)
        {
            UpdateTileUnionGizmos(tileUnion);
        }

        public void OnPathGizmoChanged(bool value)
        {
            showPathGizmo = value;
            tileBuilder.TileUnionsWithStash.ForEach(UpdateTileUnionGizmos);
        }

        public void OnShowHiddenTilesChanged(bool value)
        {
            showHiddenTilesCube = value;
            tileBuilder.TileUnionsWithStash.ForEach(UpdateTileUnionGizmos);
        }

        private void UpdateTileUnionGizmos(TileUnionImpl tileUnion)
        {
            if (
                !tileUnion.IsAllWithMark(RoomTileLabel.FreeSpace)
                && !tileUnion.IsAllWithMark(RoomTileLabel.Outside)
            )
            {
                tileUnion.SetPathGizmo(showPathGizmo);
            }
            if (tileUnion.IsAllWithMark(RoomTileLabel.Outside))
            {
                tileUnion.SetCenterCube(showHiddenTilesCube);
            }
        }
    }
}
#endif
