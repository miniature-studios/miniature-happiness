#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace TileUnion.Inspector
{
    [CustomEditor(typeof(TileUnion))]
    public class TileUnionInspector : Editor
    {
        private TileUnion tile_union;

        public override void OnInspectorGUI()
        {
            tile_union = serializedObject.targetObject as TileUnion;

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Show rotating"))
            {
                tile_union.CreateCache();
                _ = tile_union.StartCoroutine(RotateAfterWait(0));
            }
            EditorGUILayout.EndHorizontal();

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }

        private IEnumerator RotateAfterWait(int counter)
        {
            Debug.Log($"Rotation {counter}");
            tile_union.SetRotation(counter);
            counter++;
            yield return new WaitForSecondsRealtime(1);
            if (counter <= 3)
            {
                _ = tile_union.StartCoroutine(RotateAfterWait(counter));
            }
            else if (counter == 4)
            {
                tile_union.SetRotation(0);
                Debug.Log("End Rotation");
            }
        }
    }
}
#endif
