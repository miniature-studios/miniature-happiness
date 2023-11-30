#if UNITY_EDITOR
using Common;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TileUnion.Tile
{
    public partial class TileImpl
    {
        [ReadOnly]
        public bool ShowDirectionGizmo = false;

        [ReadOnly]
        public bool ShowPathGizmo = false;

        private float lineThickness = 4;
        private Vector3 right = new(0, 0, -1);
        private Vector3 left = new(0, 0, 1);
        private Vector3 up = new(1, 0, 0);
        private Vector3 down = new(-1, 0, 0);
        private GameObject centerCube = null;

        public void SetCubeInCenter(bool value)
        {
            switch (centerCube == null, value)
            {
                case (true, true):
                    centerCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    centerCube.transform.parent = transform.Find("Center").transform;
                    break;
                case (false, false):
                    Destroy(centerCube);
                    break;
                default:
                    break;
            }
        }

        private void OnDrawGizmos()
        {
            if (ShowDirectionGizmo)
            {
                Handles.Label(
                    transform.position + transform.TransformDirection(right * 2),
                    "Right"
                );
                Handles.Label(transform.position + transform.TransformDirection(left * 2), "Left");
                Handles.Label(transform.position + transform.TransformDirection(up * 2), "Up");
                Handles.Label(transform.position + transform.TransformDirection(down * 2), "Down");
            }
            if (ShowPathGizmo)
            {
                foreach (Direction dir in GetPassableDirections())
                {
                    switch (dir)
                    {
                        case Direction.Up:
                            Handles.DrawLine(
                                transform.position + Vector3.up,
                                transform.position + (up * 2.5f) + Vector3.up,
                                lineThickness
                            );
                            break;
                        case Direction.Right:
                            Handles.DrawLine(
                                transform.position + Vector3.up,
                                transform.position + (right * 2.5f) + Vector3.up,
                                lineThickness
                            );
                            break;
                        case Direction.Down:
                            Handles.DrawLine(
                                transform.position + Vector3.up,
                                transform.position + (down * 2.5f) + Vector3.up,
                                lineThickness
                            );
                            break;
                        case Direction.Left:
                            Handles.DrawLine(
                                transform.position + Vector3.up,
                                transform.position + (left * 2.5f) + Vector3.up,
                                lineThickness
                            );
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TileImpl))]
    public class TileInspector : Editor
    {
        private void ShowWallsFilling(TileImpl tile)
        {
            _ = EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Fill walls"))
            {
                tile.RawWalls = new();
                foreach (Direction direction in Direction.Up.GetCircle90())
                {
                    WallCollection wallsCollection = new() { Place = direction, Handlers = new() };
                    foreach (WallType wallType in Enum.GetValues(typeof(WallType)).Cast<WallType>())
                    {
                        wallsCollection.Handlers.Add(
                            new WallPrefabHandler() { Type = wallType, Prefab = null }
                        );
                    }
                    tile.RawWalls.Add(wallsCollection);
                }
                tile.CreateWallsCache();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowCornersFilling(TileImpl tile)
        {
            _ = EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Fill corners"))
            {
                tile.Corners = new();
                foreach (Direction direction in Direction.RightUp.GetCircle90())
                {
                    CornerCollection cornersCollection =
                        new() { Place = direction, Handlers = new() };
                    foreach (
                        CornerType cornerType in Enum.GetValues(typeof(CornerType))
                            .Cast<CornerType>()
                    )
                    {
                        cornersCollection.Handlers.Add(
                            new CornerPrefabHandler() { Type = cornerType, Prefab = null }
                        );
                    }
                    tile.Corners.Add(cornersCollection);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ShowPrefabsFilling(TileImpl tile)
        {
            _ = EditorGUILayout.BeginHorizontal();
            GUILayout.Label("PRESS THIS BUTTON ONLY IF ROTATION IS ZERO!");
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Fill tile with prefabs"))
            {
                if (tile.Rotation == 0)
                {
                    GameObject wallsHandler;
                    if (tile.transform.Find("Walls") == null)
                    {
                        wallsHandler = new GameObject("Walls");
                        wallsHandler.transform.parent = tile.gameObject.transform;
                    }
                    else
                    {
                        wallsHandler = tile.transform.Find("Walls").gameObject;
                    }
                    while (wallsHandler.transform.childCount > 0)
                    {
                        DestroyImmediate(wallsHandler.transform.GetChild(0).gameObject);
                    }

                    foreach (WallCollection wallCollection in tile.RawWalls)
                    {
                        float degrees = wallCollection.Place.GetDegrees();
                        foreach (WallPrefabHandler handler in wallCollection.Handlers)
                        {
                            WallPrefabHandler prefabHandler = tile.WallPrefabHandlers.Find(
                                x => x.Type == handler.Type
                            );
                            if (prefabHandler != null)
                            {
                                if (handler.Prefab != null)
                                {
                                    DestroyImmediate(handler.Prefab);
                                }

                                handler.Prefab =
                                    PrefabUtility.InstantiatePrefab(
                                        prefabHandler.Prefab,
                                        wallsHandler.transform
                                    ) as GameObject;

                                handler.Prefab.transform.SetPositionAndRotation(
                                    tile.transform.position,
                                    prefabHandler.Prefab.transform.rotation
                                );

                                handler.Prefab.transform.Rotate(new(0, degrees, 0));
                                handler.Prefab.SetActive(false);
                                handler.Prefab.name =
                                    $"Wall - {handler.Type} - {wallCollection.Place} -| "
                                    + handler.Prefab.name;
                            }
                            else
                            {
                                Debug.LogError($"Cannot find prefab for {handler.Type}");
                            }
                        }
                    }

                    GameObject cornersHandler;
                    if (tile.transform.Find("Corners") == null)
                    {
                        cornersHandler = new GameObject("Corners");
                        cornersHandler.transform.parent = tile.gameObject.transform;
                    }
                    else
                    {
                        cornersHandler = tile.transform.Find("Corners").gameObject;
                    }
                    while (cornersHandler.transform.childCount > 0)
                    {
                        DestroyImmediate(cornersHandler.transform.GetChild(0).gameObject);
                    }

                    foreach (CornerCollection cornerCollection in tile.Corners)
                    {
                        float degrees = cornerCollection.Place.GetDegrees() - 45;
                        foreach (CornerPrefabHandler handler in cornerCollection.Handlers)
                        {
                            CornerPrefabHandler prefabHandler = tile.CornerPrefabHandlers.Find(
                                x => x.Type == handler.Type
                            );
                            if (prefabHandler != null)
                            {
                                if (handler.Prefab != null)
                                {
                                    DestroyImmediate(handler.Prefab);
                                }

                                handler.Prefab =
                                    PrefabUtility.InstantiatePrefab(
                                        prefabHandler.Prefab,
                                        cornersHandler.transform
                                    ) as GameObject;

                                handler.Prefab.transform.SetPositionAndRotation(
                                    tile.transform.position,
                                    prefabHandler.Prefab.transform.rotation
                                );

                                handler.Prefab.transform.Rotate(new(0, degrees, 0));
                                handler.Prefab.SetActive(false);
                                handler.Prefab.name =
                                    $"Corner - {handler.Type} - {cornerCollection.Place} -| "
                                    + handler.Prefab.name;
                            }
                            else
                            {
                                Debug.LogError($"Cannot find prefab for {handler.Type}");
                            }
                        }
                    }

                    GameObject centerHandler;
                    if (tile.transform.Find("Center") == null)
                    {
                        centerHandler = new GameObject("Center");
                        centerHandler.transform.parent = tile.gameObject.transform;
                    }
                    else
                    {
                        centerHandler = tile.transform.Find("Center").gameObject;
                    }
                    while (centerHandler.transform.childCount > 0)
                    {
                        DestroyImmediate(centerHandler.transform.GetChild(0).gameObject);
                    }

                    foreach (GameObject centerObject in tile.CenterPrefabs)
                    {
                        _ = PrefabUtility.InstantiatePrefab(centerObject, centerHandler.transform);
                    }

                    EditorUtility.SetDirty(tile);
                }
                else if (tile.Rotation != 0)
                {
                    Debug.LogError("Press this only if rotation is zero!");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            TileImpl tile = serializedObject.targetObject as TileImpl;

            ShowWallsFilling(tile);
            ShowCornersFilling(tile);
            ShowPrefabsFilling(tile);

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
