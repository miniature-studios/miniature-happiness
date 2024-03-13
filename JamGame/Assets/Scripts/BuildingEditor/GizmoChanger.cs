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
            UpdateTileUnionGizmos(tileUnion);
        }

        public void SwitchShowTileDirectionGizmo()
        {
            showDirectionGizmo ^= true;
            tileBuilder.TileUnionsWithStash.ForEach(UpdateTileUnionGizmos);
        }

        public void SwitchShowTilePathGizmo()
        {
            showPathGizmo ^= true;
            tileBuilder.TileUnionsWithStash.ForEach(UpdateTileUnionGizmos);
        }

        public void SwitchShowHiddenTilesCube()
        {
            showHiddenTilesCube ^= true;
            tileBuilder.TileUnionsWithStash.ForEach(UpdateTileUnionGizmos);
        }

        private void UpdateTileUnionGizmos(TileUnionImpl tileUnion)
        {
            tileUnion.SetDirectionsGizmo(showDirectionGizmo);
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
