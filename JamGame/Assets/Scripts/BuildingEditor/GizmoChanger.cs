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
        private bool showDirectionGizmo = false;

        [ReadOnly]
        [SerializeField]
        private bool showPathGizmo = false;

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
            showDirectionGizmo = !showDirectionGizmo;
            tileBuilder.TileUnionsWithStash.ForEach(SetDirectionsGizmo);
        }

        public void SwitchShowTilePathGizmo()
        {
            showPathGizmo = !showPathGizmo;
            tileBuilder.TileUnionsWithStash.ForEach(SetPathGizmo);
        }

        public void SwitchShowHiddenTilesCube()
        {
            showHiddenTilesCube = !showHiddenTilesCube;
            tileBuilder.TileUnionsWithStash.ForEach(SetCenterCube);
        }

        private void SetDirectionsGizmo(TileUnionImpl tileUnion)
        {
            tileUnion.SetDirectionsGizmo(showDirectionGizmo);
        }

        private void SetPathGizmo(TileUnionImpl tileUnion)
        {
            if (
                !tileUnion.IsAllWithMark(RoomTileLabel.FreeSpace)
                && !tileUnion.IsAllWithMark(RoomTileLabel.Outside)
            )
            {
                tileUnion.SetPathGizmo(showPathGizmo);
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
