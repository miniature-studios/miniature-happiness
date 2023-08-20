using Level.Room;
using Pickle;
using System.Linq;
using TileUnion;
using UnityEditor;
using UnityEngine;

namespace TileBuilder
{
    public partial class TileBuilderImpl
    {
        [HideInInspector]
        public GameObject LoadingPrefab;

        [HideInInspector]
        public string SavingName = "SampleBuilding";

        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel StairsPrefab;

        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel WindowPrefab;

        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel OutdoorPrefab;

        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel CorridorPrefab;

        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel WorkingPlaceFree;

        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel WorkingPlace;

        [HideInInspector]
        public int SquareSideLength = 30;

        [HideInInspector]
        public bool LoadFromSceneComposition;

        [HideInInspector]
        public bool ShowTileDirectionGizmo;

        [HideInInspector]
        public bool ShowTilePathGizmo;

        [HideInInspector]
        public bool ShowTileFreeSpaceCube;

        public void EditorStart()
        {
            if (LoadFromSceneComposition && LoadingPrefab != null)
            {
                while (TileUnionDictionary.Values.Count() > 0)
                {
                    TileUnionImpl value = TileUnionDictionary.Values.Last();
                    _ = DeleteTile(value);
                }
                TileUnionDictionary.Clear();
                LoadSceneComposition();
            }
        }

        public void LoadSceneComposition()
        {
            if (RootObject != null)
            {
                DestroyImmediate(RootObject);
            }
            RootObject = Instantiate(LoadingPrefab, transform);
            foreach (TileUnionImpl union in RootObject.GetComponentsInChildren<TileUnionImpl>())
            {
                foreach (Vector2Int pos in union.TilesPositions)
                {
                    TileUnionDictionary.Add(pos, union);
                }
            }
            UpdateAllTiles();
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TileBuilderImpl))]
    public partial class TileBuilderInspector : Editor
    {
        private TileBuilderImpl tileBuilder;

        private void Awake()
        {
            tileBuilder = serializedObject.targetObject as TileBuilderImpl;
            tileBuilder.OnTileUnionCreated += OnTileUnionCreated;
        }

        private void OnTileUnionCreated(TileUnionImpl tileUnionImpl)
        {
            if (tileBuilder.ShowTileDirectionGizmo)
            {
                tileUnionImpl.SetDirectionsGizmo(true);
            }
            if (
                tileBuilder.ShowTilePathGizmo
                && !tileUnionImpl.IsAllWithMark("Freespace")
                && !tileUnionImpl.IsAllWithMark("Outside")
            )
            {
                tileUnionImpl.SetPathGizmo(true);
            }
            if (tileBuilder.ShowTileFreeSpaceCube && tileUnionImpl.IsAllWithMark("Freespace"))
            {
                tileUnionImpl.SetCenterCube(true);
            }
        }

        public void DeleteAllTiles()
        {
            if (tileBuilder.RootObject != null)
            {
                DestroyImmediate(tileBuilder.RootObject);
            }
            tileBuilder.RootObject = Instantiate(
                new GameObject("Root object"),
                tileBuilder.transform
            );
            while (tileBuilder.TileUnionDictionary.Values.Contains(null))
            {
                _ = tileBuilder.TileUnionDictionary.Remove(
                    tileBuilder.TileUnionDictionary.First(x => x.Value == null).Key
                );
            }
        }

        public override void OnInspectorGUI()
        {
            ShowLocationBuildingButtons(tileBuilder);
            DisplaySaveLoadTiles(tileBuilder);
            DisplayDebuggingTools(tileBuilder);

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }

        private void DisplayDebuggingTools(TileBuilderImpl tile_builder)
        {
            bool buffer_bool;

            _ = EditorGUILayout.BeginHorizontal();
            buffer_bool = EditorGUILayout.Toggle(
                "Show Tile Direction Gizmo?",
                tile_builder.ShowTileDirectionGizmo
            );
            if (buffer_bool != tile_builder.ShowTileDirectionGizmo)
            {
                foreach (
                    TileUnionImpl tileUnion in tile_builder.TileUnionDictionary.Values.Distinct()
                )
                {
                    tileUnion.SetDirectionsGizmo(!tile_builder.ShowTileDirectionGizmo);
                }
            }
            tile_builder.ShowTileDirectionGizmo = buffer_bool;
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            buffer_bool = EditorGUILayout.Toggle(
                "Show Tile FreeSpace Model?",
                tile_builder.ShowTileFreeSpaceCube
            );
            if (buffer_bool != tile_builder.ShowTileFreeSpaceCube)
            {
                foreach (
                    TileUnionImpl tileUnion in tile_builder.TileUnionDictionary.Values.Distinct()
                )
                {
                    if (tileUnion.IsAllWithMark("Freespace"))
                    {
                        tileUnion.SetCenterCube(!tile_builder.ShowTileFreeSpaceCube);
                    }
                }
            }
            tile_builder.ShowTileFreeSpaceCube = buffer_bool;
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            buffer_bool = EditorGUILayout.Toggle(
                "Show Tile Path Gizmo?",
                tile_builder.ShowTilePathGizmo
            );
            if (buffer_bool != tile_builder.ShowTilePathGizmo)
            {
                foreach (
                    TileUnionImpl tileUnion in tile_builder.TileUnionDictionary.Values.Distinct()
                )
                {
                    if (
                        !tileUnion.IsAllWithMark("Freespace") && !tileUnion.IsAllWithMark("Outside")
                    )
                    {
                        tileUnion.SetPathGizmo(!tile_builder.ShowTilePathGizmo);
                    }
                }
            }
            tile_builder.ShowTilePathGizmo = buffer_bool;
            EditorGUILayout.EndHorizontal();
        }

        private void DisplaySaveLoadTiles(TileBuilderImpl tile_builder)
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
                "Saving name: ",
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
                tile_builder.LoadSceneComposition();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowLocationBuildingButtons(TileBuilderImpl tile_builder)
        {
            _ = EditorGUILayout.BeginHorizontal();
            tile_builder.SquareSideLength = EditorGUILayout.IntField(
                "Square side length: ",
                tile_builder.SquareSideLength
            );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create random building"))
            {
                int x = 0;
                int y = 0;
                DeleteAllTiles();
                for (
                    int i = 0;
                    i < tile_builder.SquareSideLength * tile_builder.SquareSideLength;
                    i++
                )
                {
                    float value = Random.value * 100;
                    if (value < 50)
                    {
                        tile_builder.CreateTileAndBind(tile_builder.FreeSpace, new(x, y), 0);
                    }
                    else if (value is > 50 and < 65)
                    {
                        tile_builder.CreateTileAndBind(tile_builder.StairsPrefab, new(x, y), 0);
                    }
                    else if (value is > 65 and < 80)
                    {
                        tile_builder.CreateTileAndBind(tile_builder.WindowPrefab, new(x, y), 0);
                    }
                    else if (value > 80)
                    {
                        tile_builder.CreateTileAndBind(tile_builder.OutdoorPrefab, new(x, y), 0);
                    }
                    y++;
                    if (y >= tile_builder.SquareSideLength)
                    {
                        y = 0;
                        x++;
                    }
                }
            }
            if (GUILayout.Button("Create normal building"))
            {
                DeleteAllTiles();
                for (int i = 0; i < 9; i++)
                {
                    tile_builder.CreateTileAndBind(tile_builder.OutdoorPrefab, new(0, i), 0);
                }

                for (int i = 0; i < 8; i++)
                {
                    if (i == 1)
                    {
                        tile_builder.CreateTileAndBind(tile_builder.StairsPrefab, new(i + 1, 0), 0);
                    }
                    else
                    {
                        tile_builder.CreateTileAndBind(
                            tile_builder.OutdoorPrefab,
                            new(i + 1, 0),
                            0
                        );
                    }

                    for (int j = 0; j < 7; j++)
                    {
                        if (j == 2)
                        {
                            tile_builder.CreateTileAndBind(
                                tile_builder.CorridorPrefab,
                                new(i + 1, j + 1),
                                0
                            );
                        }
                        else if (j == 3)
                        {
                            tile_builder.CreateTileAndBind(
                                tile_builder.WorkingPlace,
                                new(i + 1, j + 1),
                                0
                            );
                        }
                        else if (j == 4)
                        {
                            tile_builder.CreateTileAndBind(
                                tile_builder.WorkingPlaceFree,
                                new(i + 1, j + 1),
                                0
                            );
                        }
                        else
                        {
                            tile_builder.CreateTileAndBind(
                                tile_builder.FreeSpace,
                                new(i + 1, j + 1),
                                0
                            );
                        }
                    }
                    tile_builder.CreateTileAndBind(tile_builder.OutdoorPrefab, new(i + 1, 8), 0);
                }
                for (int i = 0; i < 9; i++)
                {
                    tile_builder.CreateTileAndBind(tile_builder.OutdoorPrefab, new(9, i), 0);
                }
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Scene"))
            {
                DeleteAllTiles();
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add 4 Tiles"))
            {
                DeleteAllTiles();
                tile_builder.CreateTileAndBind(tile_builder.OutdoorPrefab, new(0, 0), 0);
                tile_builder.CreateTileAndBind(tile_builder.OutdoorPrefab, new(0, 1), 0);
                tile_builder.CreateTileAndBind(tile_builder.WorkingPlaceFree, new(1, 0), 0);
                tile_builder.CreateTileAndBind(tile_builder.WorkingPlace, new(1, 1), 0);
            }
            if (GUILayout.Button("Update All"))
            {
                tile_builder.UpdateAllTiles();
            }
            EditorGUILayout.EndHorizontal();
        }
    }
#endif
}
