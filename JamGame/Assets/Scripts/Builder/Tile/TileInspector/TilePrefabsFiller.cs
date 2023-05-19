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
            GameObject walls_hadler;
            if (tile.transform.Find("Walls") == null)
            {
                walls_hadler = new GameObject("Walls");
                walls_hadler.transform.parent = tile.gameObject.transform;
            }
            else
            {
                walls_hadler = tile.transform.Find("Walls").gameObject;
            }

            foreach (WallCollection wall_collection in tile.Walls)
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
                        handler.Prefab = Instantiate(prefab_handler.Prefab, tile.transform.position, prefab_handler.Prefab.transform.rotation, walls_hadler.transform);
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

            GameObject corners_hadler;
            if (tile.transform.Find("Corners") == null)
            {
                corners_hadler = new GameObject("Corners");
                corners_hadler.transform.parent = tile.gameObject.transform;
            }
            else
            {
                corners_hadler = tile.transform.Find("Corners").gameObject;
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

                        handler.Prefab = Instantiate(prefab_handler.Prefab, tile.transform.position, prefab_handler.Prefab.transform.rotation, corners_hadler.transform);
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

            GameObject center_hadler;
            if (tile.transform.Find("Center") == null)
            {
                center_hadler = new GameObject("Center");
                center_hadler.transform.parent = tile.gameObject.transform;
            }
            else
            {
                center_hadler = tile.transform.Find("Center").gameObject;
            }
        }

        EditorGUILayout.EndHorizontal();
    }
}
#endif