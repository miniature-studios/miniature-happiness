#if UNITY_EDITOR
using Location;
using UnityEditor;
using UnityEngine;

namespace TileUnion
{
    public partial class TileUnionImpl
    {
        public void SetDirectionsGizmo(bool value)
        {
            foreach (Tile.TileImpl tile in Tiles)
            {
                tile.ShowDirectionGizmo = value;
            }
        }

        public void SetPathGizmo(bool value)
        {
            foreach (Tile.TileImpl tile in Tiles)
            {
                tile.ShowPathGizmo = value;
            }
        }

        public void SetCenterCube(bool value)
        {
            foreach (Tile.TileImpl tile in Tiles)
            {
                tile.SetCubeInCenter(value);
            }
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
    }

    [CustomEditor(typeof(TileUnionImpl))]
    public class TileUnionInspector : Editor
    {

        public override void OnInspectorGUI()
        {
            TileUnionImpl tile_union = serializedObject.targetObject as TileUnionImpl;

            _ = EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Must be pressed before usage!");
            EditorGUILayout.EndHorizontal();
            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Update cache"))
            {
                tile_union.CreateCache();
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set rotation 0 from cache"))
            {
                tile_union.SetRotation(0);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set rotation 1 from cache"))
            {
                tile_union.SetRotation(1);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set rotation 2 from cache"))
            {
                tile_union.SetRotation(2);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set rotation 3 from cache"))
            {
                tile_union.SetRotation(3);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            GUILayout.Label("For testing");
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Isolate Update"))
            {
                tile_union.IsolateUpdate();
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show Directions Gizmo"))
            {
                tile_union.SetDirectionsGizmo(true);
            }
            if (GUILayout.Button("Hide Directions Gizmo"))
            {
                tile_union.SetDirectionsGizmo(false);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show Path Gizmo"))
            {
                tile_union.SetPathGizmo(true);
            }
            if (GUILayout.Button("Hide Path Gizmo"))
            {
                tile_union.SetPathGizmo(false);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show Need Provider Gizmo"))
            {
                tile_union.DrawNeedProviderGizmo = true;
            }
            if (GUILayout.Button("Hide Need Provider Gizmo"))
            {
                tile_union.DrawNeedProviderGizmo = false;
            }
            EditorGUILayout.EndHorizontal();

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
