#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public partial class TileBuilderInspector
{
    public partial void ShowTilesSaveLoading(TileBuilder tile_builder)
    {
        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.loadingPrefab = (GameObject)EditorGUILayout.ObjectField("Loading prefab: ", tile_builder.loadingPrefab, typeof(GameObject), false);
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        tile_builder.savingName = EditorGUILayout.TextField("Saveing name: ", tile_builder.savingName);
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save scene composition into prefab."))
        {
            string localPath = "Assets/Prefabs/SceneCompositions/" + tile_builder.savingName + ".prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            _ = PrefabUtility.SaveAsPrefabAsset(tile_builder.RootObject, localPath, out bool prefabSuccess);
            if (prefabSuccess == true)
            {
                Debug.Log("Prefab was saved successfully");
            }
            else
            {
                Debug.Log("Prefab failed to save");
            }
        }
        EditorGUILayout.EndHorizontal();

        _ = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load scene composition from prefab."))
        {
            tile_builder.LoadSceneComposition(tile_builder.loadingPrefab);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
