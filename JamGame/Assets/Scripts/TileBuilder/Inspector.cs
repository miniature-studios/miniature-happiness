#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace TileBuilder.Inspector
{
    [CustomEditor(typeof(TileBuilder))]
    public partial class TileBuilderInspector : Editor
    {
        [HideInInspector]
        public GameObject LoadingPrefab;

        [HideInInspector]
        public string SavingName = "SampleBuilding";

        [HideInInspector]
        public TileUnion StairsPrefab;

        [HideInInspector]
        public TileUnion WindowPrefab;

        [HideInInspector]
        public TileUnion OutdoorPrefab;

        [HideInInspector]
        public TileUnion CorridoorPrefab;

        [HideInInspector]
        public TileUnion WorkingPlaceFree;

        [HideInInspector]
        public TileUnion WorkingPlace;

        [HideInInspector]
        public int SquareSideLength = 30;

        [HideInInspector]
        public bool LoadFromSceneComposition;

        public override void OnInspectorGUI()
        {
            TileBuilder tile_builder = serializedObject.targetObject as TileBuilder;

            ShowLocationBuildingButtons(tile_builder);
            DisplaySaveLoadTiles(tile_builder);

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }

        private void DisplaySaveLoadTiles(TileBuilder tile_builder)
        {
            _ = EditorGUILayout.BeginHorizontal();
            tile_builder.LoadingPrefab = (GameObject)
                EditorGUILayout.ObjectField(
                    "Loading prefab: ",
                    tile_builder.LoadingPrefab,
                    typeof(GameObject),
                    false
                );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            tile_builder.SavingName = EditorGUILayout.TextField(
                "Saveing name: ",
                tile_builder.SavingName
            );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save scene composition into prefab."))
            {
                string localPath =
                    "Assets/Prefabs/SceneCompositions/" + tile_builder.SavingName + ".prefab";
                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                _ = PrefabUtility.SaveAsPrefabAsset(
                    tile_builder.RootObject,
                    localPath,
                    out bool prefabSuccess
                );
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
            tile_builder.LoadFromSceneComposition = EditorGUILayout.Toggle(
                "Load from prefab on start?",
                tile_builder.LoadFromSceneComposition
            );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load scene composition from prefab."))
            {
                tile_builder.LoadSceneComposition(tile_builder.LoadingPrefab);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowLocationBuildingButtons(TileBuilder tile_builder)
        {
            _ = EditorGUILayout.BeginHorizontal();
            tile_builder.SquareSideLength = EditorGUILayout.IntField(
                "Square side length: ",
                tile_builder.SquareSideLength
            );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            tile_builder.StairsPrefab = (TileUnion)
                EditorGUILayout.ObjectField(
                    "Stairs prefab: ",
                    tile_builder.StairsPrefab,
                    typeof(TileUnion),
                    true
                );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            tile_builder.WindowPrefab = (TileUnion)
                EditorGUILayout.ObjectField(
                    "Window prefab: ",
                    tile_builder.WindowPrefab,
                    typeof(TileUnion),
                    true
                );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            tile_builder.OutdoorPrefab = (TileUnion)
                EditorGUILayout.ObjectField(
                    "Outdoor prefab: ",
                    tile_builder.OutdoorPrefab,
                    typeof(TileUnion),
                    true
                );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            tile_builder.CorridoorPrefab = (TileUnion)
                EditorGUILayout.ObjectField(
                    "Corridoor prefab: ",
                    tile_builder.CorridoorPrefab,
                    typeof(TileUnion),
                    true
                );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            tile_builder.WorkingPlaceFree = (TileUnion)
                EditorGUILayout.ObjectField(
                    "Working place free prefab: ",
                    tile_builder.WorkingPlaceFree,
                    typeof(TileUnion),
                    true
                );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            tile_builder.WorkingPlace = (TileUnion)
                EditorGUILayout.ObjectField(
                    "Working place prefab: ",
                    tile_builder.WorkingPlace,
                    typeof(TileUnion),
                    true
                );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create random building"))
            {
                tile_builder.CreateRandomBuilding();
            }
            if (GUILayout.Button("Create normal building"))
            {
                tile_builder.CreateNormalBuilding();
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Scene"))
            {
                tile_builder.DeleteAllTiles();
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add 4 Tiles"))
            {
                tile_builder.CreateFourTiles();
            }
            if (GUILayout.Button("Update All"))
            {
                tile_builder.UpdateAllTiles();
            }
            EditorGUILayout.EndHorizontal();
        }

        public void CreateRandomBuilding()
        {
            int x = 0;
            int y = 0;
            DeleteAllTiles();
            for (int i = 0; i < SquareSideLength * SquareSideLength; i++)
            {
                float value = Random.value * 100;
                if (value < 50)
                {
                    CreateTileAndBind(freespacePrefab, new(x, y), 0);
                }
                else if (value is > 50 and < 65)
                {
                    CreateTileAndBind(StairsPrefab, new(x, y), 0);
                }
                else if (value is > 65 and < 80)
                {
                    CreateTileAndBind(WindowPrefab, new(x, y), 0);
                }
                else if (value > 80)
                {
                    CreateTileAndBind(OutdoorPrefab, new(x, y), 0);
                }
                y++;
                if (y >= SquareSideLength)
                {
                    y = 0;
                    x++;
                }
            }
        }

        public void CreateNormalBuilding()
        {
            DeleteAllTiles();
            for (int i = 0; i < 9; i++)
            {
                CreateTileAndBind(OutdoorPrefab, new(0, i), 0);
            }

            for (int i = 0; i < 8; i++)
            {
                if (i == 1)
                {
                    CreateTileAndBind(StairsPrefab, new(i + 1, 0), 0);
                }
                else
                {
                    CreateTileAndBind(OutdoorPrefab, new(i + 1, 0), 0);
                }

                for (int j = 0; j < 7; j++)
                {
                    if (j == 2)
                    {
                        CreateTileAndBind(CorridoorPrefab, new(i + 1, j + 1), 0);
                    }
                    else if (j == 3)
                    {
                        CreateTileAndBind(WorkingPlace, new(i + 1, j + 1), 0);
                    }
                    else if (j == 4)
                    {
                        CreateTileAndBind(WorkingPlaceFree, new(i + 1, j + 1), 0);
                    }
                    else
                    {
                        CreateTileAndBind(freespacePrefab, new(i + 1, j + 1), 0);
                    }
                }
                CreateTileAndBind(OutdoorPrefab, new(i + 1, 8), 0);
            }
            for (int i = 0; i < 9; i++)
            {
                CreateTileAndBind(OutdoorPrefab, new(9, i), 0);
            }
        }

        public void CreateFourTiles()
        {
            DeleteAllTiles();
            CreateTileAndBind(OutdoorPrefab, new(0, 0), 0);
            CreateTileAndBind(OutdoorPrefab, new(0, 1), 0);
            CreateTileAndBind(WorkingPlaceFree, new(1, 0), 0);
            CreateTileAndBind(WorkingPlace, new(1, 1), 0);
        }
    }
}
#endif
