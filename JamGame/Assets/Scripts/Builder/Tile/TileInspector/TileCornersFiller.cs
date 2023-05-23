#if UNITY_EDITOR
using Common;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

public partial class TileInspector
{
    public partial void ShowCornersFilling(Tile tile)
    {
        _ = EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Fill corners"))
        {
            tile.Corners = new();
            foreach (Direction direction in Direction.RightUp.GetCircle90())
            {
                CornerCollection corners_collection = new()
                {
                    Place = direction,
                    Handlers = new()
                };
                foreach (TileCornerType corner_type in Enum.GetValues(typeof(TileCornerType)).Cast<TileCornerType>())
                {
                    corners_collection.Handlers.Add(new CornerPrefabHandler() { Type = corner_type, Prefab = null });
                }
                tile.Corners.Add(corners_collection);
            }
        }

        EditorGUILayout.EndHorizontal();
    }
}
#endif