using Common;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InternalPathCollection))]
public class InternalPathCollectionEditor : Editor
{
    float room_size = 4.1f;

    public override void OnInspectorGUI()
    {
        var pathCollection = serializedObject.targetObject as InternalPathCollection;

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Room size ");
        room_size = EditorGUILayout.FloatField(room_size);
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Fill with default paths"))
        {
            var straignt_pairs = new List<(Direction, Direction)>() {
                (Direction.Up, Direction.Down),
                (Direction.Down, Direction.Up),
                (Direction.Left, Direction.Right),
                (Direction.Right, Direction.Left)
            };

            foreach (var (from, to) in straignt_pairs)
            {
                var from_vec = GetBorderPoint(from);
                var to_vec = GetBorderPoint(to);

                var new_path = new RoomInternalPath();
                new_path.from = from;
                new_path.to = to;

                new_path.linePoints = new Vector3[] {
                    from_vec,
                    from_vec * 2.0f / 3.0f + to_vec * 1.0f / 3.0f,
                    from_vec * 1.0f / 3.0f + to_vec * 2.0f / 3.0f,
                    to_vec
                };

                pathCollection.paths.Add(new_path);
            }

            var corner_pairs = new List<(Direction, Direction)>() {
                (Direction.Up, Direction.Right),
                (Direction.Up, Direction.Left),
                (Direction.Down, Direction.Right),
                (Direction.Down, Direction.Left),
                (Direction.Left, Direction.Up),
                (Direction.Left, Direction.Down),
                (Direction.Right, Direction.Up),
                (Direction.Right, Direction.Down),
            };

            foreach (var (from, to) in corner_pairs)
            {
                var from_vec = GetBorderPoint(from);
                var to_vec = GetBorderPoint(to);

                var new_path = new RoomInternalPath();
                new_path.from = from;
                new_path.to = to;

                new_path.linePoints = new Vector3[] {
                    from_vec,
                    from_vec * 0.5f,
                    to_vec * 0.5f,
                    to_vec
                };

                pathCollection.paths.Add(new_path);
            }

            var center_marker = pathCollection.GetComponentInChildren<RoomCenterMarker>();

            if (center_marker == null)
            {
                Debug.LogError("Center marker not found! Place it as child object");
                return;
            }

            var center_pos = center_marker.transform.position;
            center_pos.y = 0;

            var directions = new List<Direction>() {
                Direction.Up,
                Direction.Down,
                Direction.Left,
                Direction.Right
            };

            foreach (var dir in directions)
            {
                var from_vec = GetBorderPoint(dir);

                var new_path = new RoomInternalPath();
                new_path.from = dir;
                new_path.to = Direction.Center;
                new_path.linePoints = new Vector3[] {
                    from_vec,
                    from_vec * 2.0f / 3.0f + center_pos * 1.0f / 3.0f,
                    from_vec * 1.0f / 3.0f + center_pos * 2.0f / 3.0f,
                    center_pos
                };
                pathCollection.paths.Add(new_path);

                var new_path_1 = new RoomInternalPath();
                new_path_1.from = Direction.Center;
                new_path_1.to = dir;
                new_path_1.linePoints = new Vector3[] {
                    center_pos,
                    from_vec * 1.0f / 3.0f + center_pos * 2.0f / 3.0f,
                    from_vec * 2.0f / 3.0f + center_pos * 1.0f / 3.0f,
                    from_vec
                };
                pathCollection.paths.Add(new_path_1);
            }

            Debug.Log("Paths are set to default");
        }

        if (GUILayout.Button("Clear all paths"))
            pathCollection.paths.Clear();

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }

    Vector3 GetBorderPoint(Direction direction)
    {
        var vec = room_size * 0.5f * (Vector2)direction.ToVector2Int();
        return new Vector3(vec.x, 0, vec.y);
    }
}