#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(Tile))]
public partial class TileInspector : Editor
{
    public partial void ShowPrefabsFilling(Tile tile);
    public override void OnInspectorGUI()
    {
        Tile tile = serializedObject.targetObject as Tile;

        ShowPrefabsFilling(tile);

        _ = DrawDefaultInspector();

        _ = serializedObject.ApplyModifiedProperties();
    }
}

#endif
