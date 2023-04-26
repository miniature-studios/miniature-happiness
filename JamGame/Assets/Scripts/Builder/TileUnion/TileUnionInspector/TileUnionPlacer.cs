#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public partial class TileUnionInspector
{
    public partial void ShowPlaceTilesButon(TileUnion tileUnion)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Do correct placing."))
        {
            foreach (var tile in tileUnion.tiles)
            {
                tile.Position = tile.Position;
                tile.Rotation = tile.Rotation;
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
