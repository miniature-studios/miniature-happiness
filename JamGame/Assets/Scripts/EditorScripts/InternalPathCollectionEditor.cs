#if UNITY_EDITOR
using Common;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorScripts
{
    [CustomEditor(typeof(InternalPathCollection))]
    public class InternalPathCollectionEditor : Editor
    {
        private float room_size = 4.1f;

        public override void OnInspectorGUI()
        {
            InternalPathCollection pathCollection = serializedObject.targetObject as InternalPathCollection;

            _ = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Room size ");
            room_size = EditorGUILayout.FloatField(room_size);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Fill with default paths"))
            {
                List<(Direction, Direction)> straignt_pairs = new() {
                (Direction.Up, Direction.Down),
                (Direction.Down, Direction.Up),
                (Direction.Left, Direction.Right),
                (Direction.Right, Direction.Left)
            };

                foreach ((Direction from, Direction to) in straignt_pairs)
                {
                    Vector3 from_vec = GetBorderPoint(from);
                    Vector3 to_vec = GetBorderPoint(to);

                    RoomInternalPath new_path = new()
                    {
                        from = from,
                        to = to,

                        linePoints = new Vector3[] {
                    from_vec,
                    (from_vec * 2.0f / 3.0f) + (to_vec * 1.0f / 3.0f),
                    (from_vec * 1.0f / 3.0f) + (to_vec * 2.0f / 3.0f),
                    to_vec
                }
                    };

                    pathCollection.paths.Add(new_path);
                }

                List<(Direction, Direction)> corner_pairs = new() {
                (Direction.Up, Direction.Right),
                (Direction.Up, Direction.Left),
                (Direction.Down, Direction.Right),
                (Direction.Down, Direction.Left),
                (Direction.Left, Direction.Up),
                (Direction.Left, Direction.Down),
                (Direction.Right, Direction.Up),
                (Direction.Right, Direction.Down),
            };

                foreach ((Direction from, Direction to) in corner_pairs)
                {
                    Vector3 from_vec = GetBorderPoint(from);
                    Vector3 to_vec = GetBorderPoint(to);

                    RoomInternalPath new_path = new()
                    {
                        from = from,
                        to = to,

                        linePoints = new Vector3[] {
                    from_vec,
                    from_vec * 0.5f,
                    to_vec * 0.5f,
                    to_vec
                }
                    };

                    pathCollection.paths.Add(new_path);
                }

                RoomCenterMarker center_marker = pathCollection.GetComponentInChildren<RoomCenterMarker>();

                if (center_marker == null)
                {
                    Debug.LogError("Center marker not found! Place it as child object");
                    return;
                }

                Vector3 center_pos = center_marker.transform.position;
                center_pos.y = 0;

                List<Direction> directions = new() {
                Direction.Up,
                Direction.Down,
                Direction.Left,
                Direction.Right
            };

                foreach (Direction dir in directions)
                {
                    Vector3 from_vec = GetBorderPoint(dir);

                    RoomInternalPath new_path = new()
                    {
                        from = dir,
                        to = Direction.Center,
                        linePoints = new Vector3[] {
                    from_vec,
                    (from_vec * 2.0f / 3.0f) + (center_pos * 1.0f / 3.0f),
                    (from_vec * 1.0f / 3.0f) + (center_pos * 2.0f / 3.0f),
                    center_pos
                }
                    };
                    pathCollection.paths.Add(new_path);

                    RoomInternalPath new_path_1 = new()
                    {
                        from = Direction.Center,
                        to = dir,
                        linePoints = new Vector3[] {
                    center_pos,
                    (from_vec * 1.0f / 3.0f) + (center_pos * 2.0f / 3.0f),
                    (from_vec * 2.0f / 3.0f) + (center_pos * 1.0f / 3.0f),
                    from_vec
                }
                    };
                    pathCollection.paths.Add(new_path_1);
                }

                Debug.Log("Paths are set to default");
            }

            if (GUILayout.Button("Clear all paths"))
            {
                pathCollection.paths.Clear();
            }

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }

        private Vector3 GetBorderPoint(Direction direction)
        {
            Vector2 vec = room_size * 0.5f * (Vector2)direction.ToVector2Int();
            return new Vector3(vec.x, 0, vec.y);
        }
    }
}
#endif