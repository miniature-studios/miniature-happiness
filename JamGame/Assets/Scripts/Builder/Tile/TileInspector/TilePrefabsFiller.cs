#if UNITY_EDITOR
using Common;
using UnityEditor;
using UnityEngine;

public partial class TileInspector
{
    public partial void ShowPrefabsFilling(Tile tile)
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
                    TileWallPrefabHandler prefab_handler = tile.ElementsHandler.WallPrefabHandlers.Find(x => x.Type == handler.Type);
                    if (prefab_handler != null)
                    {
                        if (handler.Prefab != null)
                        {
                            DestroyImmediate(handler.Prefab);
                        }
                        handler.Prefab = Instantiate(prefab_handler.Prefab, tile.transform.position, prefab_handler.Prefab.transform.rotation, walls_handler.transform);
                        handler.Prefab.transform.Rotate(new(0, degrees, 0));
                        handler.Prefab.SetActive(false);
                        handler.Prefab.name = $"Wall - {handler.Type} - {wall_collection.Place} -| " + handler.Prefab.name;
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
                    TileCornerPrefabHandler prefab_handler = tile.ElementsHandler.CornerPrefabHandlers.Find(x => x.Type == handler.Type);
                    if (prefab_handler != null)
                    {
                        if (handler.Prefab != null)
                        {
                            DestroyImmediate(handler.Prefab);
                        }

                        handler.Prefab = Instantiate(prefab_handler.Prefab, tile.transform.position, prefab_handler.Prefab.transform.rotation, corners_handler.transform);
                        handler.Prefab.transform.Rotate(new(0, degrees, 0));
                        handler.Prefab.SetActive(false);
                        handler.Prefab.name = $"Corner - {handler.Type} - {corner_collection.Place} -| " + handler.Prefab.name;
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
}
#endif