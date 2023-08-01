#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TileUnion
{
    public partial class TileUnionImpl
    {
        public void SetDireactionsGizmo(bool value)
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
            if (GUILayout.Button("Set rotation 0 from ceche"))
            {
                tile_union.SetRotation(0);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set rotation 1 from ceche"))
            {
                tile_union.SetRotation(1);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set rotation 2 from ceche"))
            {
                tile_union.SetRotation(2);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Set rotation 3 from ceche"))
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
            if (GUILayout.Button("Show Direactions Gizmo"))
            {
                tile_union.SetDireactionsGizmo(true);
            }
            if (GUILayout.Button("Hide Direactions Gizmo"))
            {
                tile_union.SetDireactionsGizmo(false);
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

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
