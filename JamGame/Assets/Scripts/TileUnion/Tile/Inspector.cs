#if UNITY_EDITOR
using System;
using System.Linq;
using Common;
using Sirenix.OdinInspector;
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

        [Button]
        private void FillWalls()
        {
            RawWalls = new();
            foreach (Direction direction in Direction.Up.GetCircle90())
            {
                WallCollection wallsCollection = new() { Place = direction, Handlers = new() };
                foreach (WallType wallType in Enum.GetValues(typeof(WallType)).Cast<WallType>())
                {
                    wallsCollection.Handlers.Add(
                        new WallPrefabHandler() { Type = wallType, Prefab = null }
                    );
                }
                RawWalls.Add(wallsCollection);
            }
            CreateWallsCache();
        }

        [Button]
        private void FillCorners()
        {
            Corners = new();
            foreach (Direction direction in Direction.RightUp.GetCircle90())
            {
                CornerCollection cornersCollection = new() { Place = direction, Handlers = new() };
                foreach (
                    CornerType cornerType in Enum.GetValues(typeof(CornerType)).Cast<CornerType>()
                )
                {
                    cornersCollection.Handlers.Add(
                        new CornerPrefabHandler() { Type = cornerType, Prefab = null }
                    );
                }
                Corners.Add(cornersCollection);
            }
        }

        [Button(DirtyOnClick = true)]
        private void FillTileWithPrefabs()
        {
            if (Rotation != 0)
            {
                Debug.LogError("Press this only if rotation is zero!");
                return;
            }

            GameObject wallsHandler;
            if (transform.Find("Walls") == null)
            {
                wallsHandler = new GameObject("Walls");
                wallsHandler.transform.parent = gameObject.transform;
            }
            else
            {
                wallsHandler = transform.Find("Walls").gameObject;
            }
            while (wallsHandler.transform.childCount > 0)
            {
                DestroyImmediate(wallsHandler.transform.GetChild(0).gameObject);
            }

            foreach (WallCollection wallCollection in RawWalls)
            {
                float degrees = wallCollection.Place.GetDegrees();
                foreach (WallPrefabHandler handler in wallCollection.Handlers)
                {
                    WallPrefabHandler prefabHandler = WallPrefabHandlers.Find(
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
                            transform.position,
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
            if (transform.Find("Corners") == null)
            {
                cornersHandler = new GameObject("Corners");
                cornersHandler.transform.parent = gameObject.transform;
            }
            else
            {
                cornersHandler = transform.Find("Corners").gameObject;
            }
            while (cornersHandler.transform.childCount > 0)
            {
                DestroyImmediate(cornersHandler.transform.GetChild(0).gameObject);
            }

            foreach (CornerCollection cornerCollection in Corners)
            {
                float degrees = cornerCollection.Place.GetDegrees() - 45;
                foreach (CornerPrefabHandler handler in cornerCollection.Handlers)
                {
                    CornerPrefabHandler prefabHandler = CornerPrefabHandlers.Find(
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
                            transform.position,
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
            if (transform.Find("Center") == null)
            {
                centerHandler = new GameObject("Center");
                centerHandler.transform.parent = gameObject.transform;
            }
            else
            {
                centerHandler = transform.Find("Center").gameObject;
            }
            while (centerHandler.transform.childCount > 0)
            {
                DestroyImmediate(centerHandler.transform.GetChild(0).gameObject);
            }

            foreach (GameObject centerObject in CenterPrefabs)
            {
                _ = PrefabUtility.InstantiatePrefab(centerObject, centerHandler.transform);
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
}
#endif
