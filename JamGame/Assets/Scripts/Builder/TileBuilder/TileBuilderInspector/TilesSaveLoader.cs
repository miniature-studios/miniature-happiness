#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public partial class TileBuilderInspector
{
    public partial void ShowTilesSaveLoading(TileBuilder tileBuilder)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save scene composition into prefab."))
        {
            string localPath = "Assets/Prefabs/SceneCompositions/" + tileBuilder.SavingName + ".prefab";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            bool prefabSuccess;
            PrefabUtility.SaveAsPrefabAsset(tileBuilder.RootObject, localPath, out prefabSuccess);
            if (prefabSuccess == true)
                Debug.Log("Prefab was saved successfully");
            else
                Debug.Log("Prefab failed to save");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load scene composition from prefab."))
        {
            tileBuilder.LoadSceneComposition(tileBuilder.LoadingPrefab);
        }
        EditorGUILayout.EndHorizontal();
    }
}
#endif
