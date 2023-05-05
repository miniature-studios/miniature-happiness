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
            GameObject wallsHadler;
            if (tile.transform.Find("Walls") == null)
            {
                wallsHadler = new GameObject("Walls");
                wallsHadler.transform.parent = tile.gameObject.transform;
            }
            else
            {
                wallsHadler = tile.transform.Find("Walls").gameObject;
            }

            foreach (WallCollection wallCollection in tile.walls)
            {
                float degrees = wallCollection.Place.GetDegrees();
                foreach (WallPrefabHandler handler in wallCollection.Handlers)
                {
                    TileWallPrefabHandler prefabHandler = tile.elementsHandler.WallPrefabHandlers.Find(x => x.Type == handler.Type);
                    if (prefabHandler != null)
                    {
                        if (handler.Prefab != null)
                        {
                            DestroyImmediate(handler.Prefab);
                        }
                        handler.Prefab = Instantiate(prefabHandler.Prefab, tile.transform.position, prefabHandler.Prefab.transform.rotation, wallsHadler.transform);
                        handler.Prefab.transform.Rotate(new(0, degrees, 0));
                        handler.Prefab.SetActive(false);
                        handler.Prefab.name = $"Wall - {handler.Type} - {wallCollection.Place} -| " + handler.Prefab.name;
                    }
                    else
                    {
                        Debug.LogError($"Cannot find prefab for {handler.Type}");
                    }
                }
            }

            GameObject cornersHadler;
            if (tile.transform.Find("Corners") == null)
            {
                cornersHadler = new GameObject("Corners");
                cornersHadler.transform.parent = tile.gameObject.transform;
            }
            else
            {
                cornersHadler = tile.transform.Find("Corners").gameObject;
            }

            foreach (CornerCollection cornerCollection in tile.corners)
            {
                float degrees = cornerCollection.Place.GetDegrees() - 45;
                foreach (CornerPrefabHandler handler in cornerCollection.Handlers)
                {
                    TileCornerPrefabHandler prefabHandler = tile.elementsHandler.CornerPrefabHandlers.Find(x => x.Type == handler.Type);
                    if (prefabHandler != null)
                    {
                        if (handler.Prefab != null)
                        {
                            DestroyImmediate(handler.Prefab);
                        }

                        handler.Prefab = Instantiate(prefabHandler.Prefab, tile.transform.position, prefabHandler.Prefab.transform.rotation, cornersHadler.transform);
                        handler.Prefab.transform.Rotate(new(0, degrees, 0));
                        handler.Prefab.SetActive(false);
                        handler.Prefab.name = $"Corner - {handler.Type} - {cornerCollection.Place} -| " + handler.Prefab.name;
                    }
                    else
                    {
                        Debug.LogError($"Cannot find prefab for {handler.Type}");
                    }
                }
            }

            GameObject centerHadler;
            if (tile.transform.Find("Center") == null)
            {
                centerHadler = new GameObject("Center");
                centerHadler.transform.parent = tile.gameObject.transform;
            }
            else
            {
                centerHadler = tile.transform.Find("Center").gameObject;
            }
        }

        EditorGUILayout.EndHorizontal();
    }
}
#endif