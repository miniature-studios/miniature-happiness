using UnityEditor;

[CustomEditor(typeof(Tile))]
public partial class TileInspector : Editor
{
    public partial void ShowPrefabsFilling(Tile tile);
    public override void OnInspectorGUI()
    {
        var tile = serializedObject.targetObject as Tile;

        ShowPrefabsFilling(tile);

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}
