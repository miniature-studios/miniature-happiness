#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public partial class TileUnionInspector
{
    public partial void ShowPlaceTilesButon(TileUnion tileUnion)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Place tiles in union to right positions."))
        {
            foreach (var tile in tileUnion.tiles)
            {
                tile.Position = tile.Position;
                tile.Rotation = tile.Rotation;
            }
            tileUnion.Rotation++;
            tileUnion.Rotation++;
            tileUnion.Rotation++;
            tileUnion.Rotation++;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Place Tile Union to right position."))
        {
            tileUnion.Position = tileUnion.Position;
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
