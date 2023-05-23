#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Tile))]
public partial class TileInspector : Editor
{
    public partial void ShowWallsFilling(Tile tile);
    public partial void ShowCornersFilling(Tile tile);
    public partial void ShowPrefabsFilling(Tile tile);
    public override void OnInspectorGUI()
    {
        Tile tile = serializedObject.targetObject as Tile;

        ShowWallsFilling(tile);
        ShowCornersFilling(tile);
        ShowPrefabsFilling(tile);

        _ = DrawDefaultInspector();

        _ = serializedObject.ApplyModifiedProperties();
    }
}
#endif