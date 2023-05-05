#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public partial class TileBuilderInspector
{
    public partial void ShowTilesSaveLoading(TileBuilder tileBuilder)
    {
        _ = EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save scene composition into prefab."))
        {
            string localPath = "Assets/Prefabs/SceneCompositions/" + tileBuilder.SavingName + ".prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            _ = PrefabUtility.SaveAsPrefabAsset(tileBuilder.RootObject, localPath, out bool prefabSuccess);
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
            tileBuilder.LoadSceneComposition(tileBuilder.LoadingPrefab);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
