#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(TileUnion))]
public partial class TileUnionInspector : Editor
{
    public partial void ShowPlaceTilesButon(TileUnion tileUnion);
    public override void OnInspectorGUI()
    {
        var tileUnion = serializedObject.targetObject as TileUnion;

        ShowPlaceTilesButon(tileUnion);

        DrawDefaultInspector();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif