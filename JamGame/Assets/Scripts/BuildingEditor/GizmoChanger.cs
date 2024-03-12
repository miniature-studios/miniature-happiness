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
        private bool showTileDirectionGizmo = false;

        [ReadOnly]
        [SerializeField]
        private bool showTilePathGizmo = false;

        [ReadOnly]
        [SerializeField]
        private bool showHiddenTilesCube = false;

        private void Awake()
        {
            tileBuilder.OnTileUnionCreated += OnTileUnionCreated;
        }

        private void OnTileUnionCreated(TileUnionImpl tileUnion)
        {
            SetDirectionsGizmo(tileUnion);
            SetPathGizmo(tileUnion);
            SetCenterCube(tileUnion);
        }

        public void SwitchShowTileDirectionGizmo()
        {
            showTileDirectionGizmo = !showTileDirectionGizmo;
            tileBuilder.TileUnionsWithStash.ForEach(SetDirectionsGizmo);
        }

        public void SwitchShowTilePathGizmo()
        {
            showTilePathGizmo = !showTilePathGizmo;
            tileBuilder.TileUnionsWithStash.ForEach(SetPathGizmo);
        }

        public void SwitchShowHiddenTilesCube()
        {
            showHiddenTilesCube = !showHiddenTilesCube;
            tileBuilder.TileUnionsWithStash.ForEach(SetCenterCube);
        }

        private void SetDirectionsGizmo(TileUnionImpl tileUnion)
        {
            tileUnion.SetDirectionsGizmo(showTileDirectionGizmo);
        }

        private void SetPathGizmo(TileUnionImpl tileUnion)
        {
            if (
                !tileUnion.IsAllWithMark(RoomTileLabel.FreeSpace)
                && !tileUnion.IsAllWithMark(RoomTileLabel.Outside)
            )
            {
                tileUnion.SetPathGizmo(showTilePathGizmo);
            }
        }

        private void SetCenterCube(TileUnionImpl tileUnion)
        {
            if (tileUnion.IsAllWithMark(RoomTileLabel.Outside))
            {
                tileUnion.SetCenterCube(showHiddenTilesCube);
            }
        }
    }
}
