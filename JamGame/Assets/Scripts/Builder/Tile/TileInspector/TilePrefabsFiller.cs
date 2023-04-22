#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using Common;

public partial class TileInspector
{
    public partial void ShowPrefabsFilling(Tile tile)
    {
        EditorGUILayout.BeginHorizontal();

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

            foreach (var wallCollection in tile.walls)
            {
                float degrees = wallCollection.Place.GetDegrees();
                foreach (var handler in wallCollection.Handlers)
                {
                    var prefabHandler = tile.elementsHandler.WallPrefabHandlers.Find(x => x.Type == handler.Type);
                    if (prefabHandler != null)
                    {
                        if (handler.Prefab != null)
                            DestroyImmediate(handler.Prefab);
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

            foreach (var cornerCollection in tile.corners)
            {
                float degrees = cornerCollection.Place.GetDegrees();
                foreach (var handler in cornerCollection.Handlers)
                {
                    var prefabHandler = tile.elementsHandler.CornerPrefabHandlers.Find(x => x.Type == handler.Type);
                    if (prefabHandler != null)
                    {
                        if (handler.Prefab != null)
                            DestroyImmediate(handler.Prefab);
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

            foreach (var object_in_center in tile.centerObjects)
            {
                var prefabHandler = tile.elementsHandler.CenterPrefabHandlers.Find(x => x.Type == object_in_center.Type);
                if (prefabHandler != null)
                {
                    if (object_in_center.Prefab != null)
                        DestroyImmediate(object_in_center.Prefab);
                    object_in_center.Prefab = Instantiate(prefabHandler.Prefab, tile.transform.position, new(), centerHadler.transform);
                    //object_in_center.Prefab.SetActive(false);
                    object_in_center.Prefab.name = $"Center - {object_in_center.Type} -| " + object_in_center.Prefab.name;
                }
                else
                {
                    Debug.LogError($"Cannot find prefab for {object_in_center.Type}");
                }
            }
        }

        EditorGUILayout.EndHorizontal();
    }
}
#endif