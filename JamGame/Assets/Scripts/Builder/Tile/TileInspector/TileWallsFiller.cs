#if UNITY_EDITOR
using Common;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class TileInspector
{
    public partial void ShowWallsFilling(Tile tile)
    {
        _ = EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Fill walls"))
        {
            tile.RawWalls = new();
            foreach (Direction direction in Direction.Up.GetCircle90())
            {
                WallCollection walls_collection = new()
                {
                    Place = direction,
                    Handlers = new()
                };
                foreach (TileWallType wall_type in Enum.GetValues(typeof(TileWallType)).Cast<TileWallType>())
                {
                    walls_collection.Handlers.Add(new WallPrefabHandler() { Type = wall_type, Prefab = null });
                }
                tile.RawWalls.Add(walls_collection);
            }
            tile.CreateWallsCache();
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
