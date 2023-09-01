#if UNITY_EDITOR
using System.Linq;
using TileUnion;
using TileUnion.SpecialRooms;
using UnityEditor;
using UnityEngine;

namespace TileBuilder
{
    public partial class TileBuilderImpl
    {
        [HideInInspector]
        public bool ShowTileDirectionGizmo;

        [HideInInspector]
        public bool ShowTilePathGizmo;

        [HideInInspector]
        public bool ShowTileFreeSpaceCube;
    }

    [CustomEditor(typeof(TileBuilderImpl))]
    public partial class TileBuilderInspector : Editor
    {
        private TileBuilderImpl tileBuilder;

        private void Awake()
        {
            tileBuilder = serializedObject.targetObject as TileBuilderImpl;
            tileBuilder.OnTileUnionCreated += OnTileUnionCreated;
        }

        public override void OnInspectorGUI()
        {
            DisplayDebuggingTools(tileBuilder);
            DisplayMeetingRoomTools(tileBuilder);

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }

        private void OnTileUnionCreated(TileUnionImpl tileUnionImpl)
        {
            if (tileBuilder.ShowTileDirectionGizmo)
            {
                tileUnionImpl.SetDirectionsGizmo(true);
            }
            if (
                tileBuilder.ShowTilePathGizmo
                && !tileUnionImpl.IsAllWithMark("Freespace")
                && !tileUnionImpl.IsAllWithMark("Outside")
            )
            {
                tileUnionImpl.SetPathGizmo(true);
            }
            if (tileBuilder.ShowTileFreeSpaceCube && tileUnionImpl.IsAllWithMark("Freespace"))
            {
                tileUnionImpl.SetCenterCube(true);
            }
        }

        private void DisplayDebuggingTools(TileBuilderImpl tileBuilder)
        {
            bool bufferBool;

            _ = EditorGUILayout.BeginHorizontal();
            bufferBool = EditorGUILayout.Toggle(
                "Show Tile Direction Gizmo?",
                tileBuilder.ShowTileDirectionGizmo
            );
            if (bufferBool != tileBuilder.ShowTileDirectionGizmo)
            {
                foreach (
                    TileUnionImpl tileUnion in tileBuilder.TileUnionDictionary.Values.Distinct()
                )
                {
                    tileUnion.SetDirectionsGizmo(!tileBuilder.ShowTileDirectionGizmo);
                }
            }
            tileBuilder.ShowTileDirectionGizmo = bufferBool;
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            bufferBool = EditorGUILayout.Toggle(
                "Show Tile FreeSpace Model?",
                tileBuilder.ShowTileFreeSpaceCube
            );
            if (bufferBool != tileBuilder.ShowTileFreeSpaceCube)
            {
                foreach (
                    TileUnionImpl tileUnion in tileBuilder.TileUnionDictionary.Values.Distinct()
                )
                {
                    if (tileUnion.IsAllWithMark("Freespace"))
                    {
                        tileUnion.SetCenterCube(!tileBuilder.ShowTileFreeSpaceCube);
                    }
                }
            }
            tileBuilder.ShowTileFreeSpaceCube = bufferBool;
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            bufferBool = EditorGUILayout.Toggle(
                "Show Tile Path Gizmo?",
                tileBuilder.ShowTilePathGizmo
            );
            if (bufferBool != tileBuilder.ShowTilePathGizmo)
            {
                foreach (
                    TileUnionImpl tileUnion in tileBuilder.TileUnionDictionary.Values.Distinct()
                )
                {
                    if (
                        !tileUnion.IsAllWithMark("Freespace") && !tileUnion.IsAllWithMark("Outside")
                    )
                    {
                        tileUnion.SetPathGizmo(!tileBuilder.ShowTilePathGizmo);
                    }
                }
            }
            tileBuilder.ShowTilePathGizmo = bufferBool;
            EditorGUILayout.EndHorizontal();
        }

        private void DisplayMeetingRoomTools(TileBuilderImpl tileBuilder)
        {
            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Grow meeting room"))
            {
                Common.Result answer = tileBuilder.GrowMeetingRoom(
                    FindObjectOfType<MeetingRoom>(),
                    FindObjectOfType<Level.Inventory.Controller>()
                );
                if (answer.Failure)
                {
                    Debug.Log(answer.Error);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
#endif
