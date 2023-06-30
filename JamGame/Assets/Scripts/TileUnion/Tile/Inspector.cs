#if UNITY_EDITOR
using Common;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace TileUnion.Tile.Inspector
{
    [CustomEditor(typeof(TileImpl))]
    public class TileInspector : Editor
    {
        private void ShowCornersFilling(TileImpl tile)
        {
            _ = EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Fill corners"))
            {
                tile.Corners = new();
                foreach (Direction direction in Direction.RightUp.GetCircle90())
                {
                    CornerCollection corners_collection =
                        new() { Place = direction, Handlers = new() };
                    foreach (
                        CornerType corner_type in Enum.GetValues(typeof(CornerType))
                            .Cast<CornerType>()
                    )
                    {
                        corners_collection.Handlers.Add(
                            new CornerPrefabHandler() { Type = corner_type, Prefab = null }
                        );
                    }
                    tile.Corners.Add(corners_collection);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ShowPrefabsFilling(TileImpl tile)
        {
            _ = EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Fill tile with prefabs"))
            {
                GameObject walls_handler;
                if (tile.transform.Find("Walls") == null)
                {
                    walls_handler = new GameObject("Walls");
                    walls_handler.transform.parent = tile.gameObject.transform;
                }
                else
                {
                    walls_handler = tile.transform.Find("Walls").gameObject;
                }
                while (walls_handler.transform.childCount > 0)
                {
                    DestroyImmediate(walls_handler.transform.GetChild(0).gameObject);
                }

                foreach (WallCollection wall_collection in tile.RawWalls)
                {
                    float degrees = wall_collection.Place.GetDegrees();
                    foreach (WallPrefabHandler handler in wall_collection.Handlers)
                    {
                        WallPrefabHandler prefab_handler = tile.WallPrefabHandlers.Find(
                            x => x.Type == handler.Type
                        );
                        if (prefab_handler != null)
                        {
                            if (handler.Prefab != null)
                            {
                                DestroyImmediate(handler.Prefab);
                            }
                            handler.Prefab = Instantiate(
                                prefab_handler.Prefab,
                                tile.transform.position,
                                prefab_handler.Prefab.transform.rotation,
                                walls_handler.transform
                            );
                            handler.Prefab.transform.Rotate(new(0, degrees, 0));
                            handler.Prefab.SetActive(false);
                            handler.Prefab.name =
                                $"Wall - {handler.Type} - {wall_collection.Place} -| "
                                + handler.Prefab.name;
                        }
                        else
                        {
                            Debug.LogError($"Cannot find prefab for {handler.Type}");
                        }
                    }
                }

                GameObject corners_handler;
                if (tile.transform.Find("Corners") == null)
                {
                    corners_handler = new GameObject("Corners");
                    corners_handler.transform.parent = tile.gameObject.transform;
                }
                else
                {
                    corners_handler = tile.transform.Find("Corners").gameObject;
                }
                while (corners_handler.transform.childCount > 0)
                {
                    DestroyImmediate(corners_handler.transform.GetChild(0).gameObject);
                }

                foreach (CornerCollection corner_collection in tile.Corners)
                {
                    float degrees = corner_collection.Place.GetDegrees() - 45;
                    foreach (CornerPrefabHandler handler in corner_collection.Handlers)
                    {
                        CornerPrefabHandler prefab_handler = tile.CornerPrefabHandlers.Find(
                            x => x.Type == handler.Type
                        );
                        if (prefab_handler != null)
                        {
                            if (handler.Prefab != null)
                            {
                                DestroyImmediate(handler.Prefab);
                            }

                            handler.Prefab = Instantiate(
                                prefab_handler.Prefab,
                                tile.transform.position,
                                prefab_handler.Prefab.transform.rotation,
                                corners_handler.transform
                            );
                            handler.Prefab.transform.Rotate(new(0, degrees, 0));
                            handler.Prefab.SetActive(false);
                            handler.Prefab.name =
                                $"Corner - {handler.Type} - {corner_collection.Place} -| "
                                + handler.Prefab.name;
                        }
                        else
                        {
                            Debug.LogError($"Cannot find prefab for {handler.Type}");
                        }
                    }
                }

                GameObject center_handler;
                if (tile.transform.Find("Center") == null)
                {
                    center_handler = new GameObject("Center");
                    center_handler.transform.parent = tile.gameObject.transform;
                }
                else
                {
                    center_handler = tile.transform.Find("Center").gameObject;
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ShowWallsFilling(TileImpl tile)
        {
            _ = EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Fill walls"))
            {
                tile.RawWalls = new();
                foreach (Direction direction in Direction.Up.GetCircle90())
                {
                    WallCollection walls_collection = new() { Place = direction, Handlers = new() };
                    foreach (
                        WallType wall_type in Enum.GetValues(typeof(WallType)).Cast<WallType>()
                    )
                    {
                        walls_collection.Handlers.Add(
                            new WallPrefabHandler() { Type = wall_type, Prefab = null }
                        );
                    }
                    tile.RawWalls.Add(walls_collection);
                }
                tile.CreateWallsCache();
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
