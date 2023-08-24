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
            TileUnionImpl tileUnion = serializedObject.targetObject as TileUnionImpl;

            _ = EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Must be pressed before usage!");
            EditorGUILayout.EndHorizontal();
            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Update cache"))
            {
                tileUnion.CreateCache();
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set rotation 0 from cache"))
            {
                tileUnion.SetRotation(0);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set rotation 1 from cache"))
            {
                tileUnion.SetRotation(1);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set rotation 2 from cache"))
            {
                tileUnion.SetRotation(2);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set rotation 3 from cache"))
            {
                tileUnion.SetRotation(3);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            GUILayout.Label("For testing");
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Isolate Update"))
            {
                tileUnion.IsolateUpdate();
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show Directions Gizmo"))
            {
                tileUnion.SetDirectionsGizmo(true);
            }
            if (GUILayout.Button("Hide Directions Gizmo"))
            {
                tileUnion.SetDirectionsGizmo(false);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show Path Gizmo"))
            {
                tileUnion.SetPathGizmo(true);
            }
            if (GUILayout.Button("Hide Path Gizmo"))
            {
                tileUnion.SetPathGizmo(false);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show Need Provider Gizmo"))
            {
                tileUnion.DrawNeedProviderGizmo = true;
            }
            if (GUILayout.Button("Hide Need Provider Gizmo"))
            {
                tileUnion.DrawNeedProviderGizmo = false;
            }
            EditorGUILayout.EndHorizontal();

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
