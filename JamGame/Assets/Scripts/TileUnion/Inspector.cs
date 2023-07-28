#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TileUnion.Inspector
{
    [CustomEditor(typeof(TileUnionImpl))]
    public class TileUnionInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            TileUnionImpl tile_union = serializedObject.targetObject as TileUnionImpl;

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

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
