#if UNITY_EDITOR
using Level.Room;
using Pickle;
using UnityEditor;
using UnityEngine;

namespace TileBuilder
{
    public partial class Controller
    {
        public TileBuilderImpl TileBuilder => tileBuilder;

        public void DeleteAllTiles()
        {
            coreModels.Clear();
            tileBuilder.DeleteAllTiles();
        }

        public void CreateTile(CoreModel coreModel, Vector2Int position, int rotation)
        {
            coreModels.Add(coreModel);
            tileBuilder.CreateTileAndBind(coreModel, position, rotation);
        }

        [Pickle]
        public GameMode GameModeToChange = GameMode.God;

        [HideInInspector]
        public int SquareSideLength = 30;

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
    }

    [CustomEditor(typeof(Controller))]
    public class Inspector : Editor
    {
        public void DisplayGameModeChange(Controller tileBuilderController)
        {
            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Change game mode"))
            {
                tileBuilderController.ChangeGameMode(tileBuilderController.GameModeToChange);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowLocationBuildingButtons(Controller tileBuilderController)
        {
            _ = EditorGUILayout.BeginHorizontal();
            tileBuilderController.SquareSideLength = EditorGUILayout.IntField(
                "Square side length: ",
                tileBuilderController.SquareSideLength
            );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create random building"))
            {
                int x = 0;
                int y = 0;
                tileBuilderController.DeleteAllTiles();
                for (
                    int i = 0;
                    i
                        < tileBuilderController.SquareSideLength
                            * tileBuilderController.SquareSideLength;
                    i++
                )
                {
                    float value = Random.value * 100;
                    if (value < 50)
                    {
                        tileBuilderController.CreateTile(
                            tileBuilderController.TileBuilder.FreeSpace,
                            new(x, y),
                            0
                        );
                    }
                    else if (value is > 50 and < 65)
                    {
                        tileBuilderController.CreateTile(
                            tileBuilderController.StairsPrefab,
                            new(x, y),
                            0
                        );
                    }
                    else if (value is > 65 and < 80)
                    {
                        tileBuilderController.CreateTile(
                            tileBuilderController.WindowPrefab,
                            new(x, y),
                            0
                        );
                    }
                    else if (value > 80)
                    {
                        tileBuilderController.CreateTile(
                            tileBuilderController.OutdoorPrefab,
                            new(x, y),
                            0
                        );
                    }
                    y++;
                    if (y >= tileBuilderController.SquareSideLength)
                    {
                        y = 0;
                        x++;
                    }
                }
            }
            if (GUILayout.Button("Create normal building"))
            {
                tileBuilderController.DeleteAllTiles();
                for (int i = 0; i < 9; i++)
                {
                    tileBuilderController.CreateTile(
                        tileBuilderController.OutdoorPrefab,
                        new(0, i),
                        0
                    );
                }

                for (int i = 0; i < 8; i++)
                {
                    if (i == 1)
                    {
                        tileBuilderController.CreateTile(
                            tileBuilderController.StairsPrefab,
                            new(i + 1, 0),
                            0
                        );
                    }
                    else
                    {
                        tileBuilderController.CreateTile(
                            tileBuilderController.OutdoorPrefab,
                            new(i + 1, 0),
                            0
                        );
                    }

                    for (int j = 0; j < 7; j++)
                    {
                        if (j == 2)
                        {
                            tileBuilderController.CreateTile(
                                tileBuilderController.CorridorPrefab,
                                new(i + 1, j + 1),
                                0
                            );
                        }
                        else if (j == 3)
                        {
                            tileBuilderController.CreateTile(
                                tileBuilderController.WorkingPlace,
                                new(i + 1, j + 1),
                                1
                            );
                        }
                        else if (j == 4)
                        {
                            tileBuilderController.CreateTile(
                                tileBuilderController.WorkingPlaceFree,
                                new(i + 1, j + 1),
                                0
                            );
                        }
                        else
                        {
                            tileBuilderController.CreateTile(
                                tileBuilderController.TileBuilder.FreeSpace,
                                new(i + 1, j + 1),
                                0
                            );
                        }
                    }
                    tileBuilderController.CreateTile(
                        tileBuilderController.OutdoorPrefab,
                        new(i + 1, 8),
                        0
                    );
                }
                for (int i = 0; i < 9; i++)
                {
                    tileBuilderController.CreateTile(
                        tileBuilderController.OutdoorPrefab,
                        new(9, i),
                        0
                    );
                }
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Scene"))
            {
                tileBuilderController.DeleteAllTiles();
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add 4 Tiles"))
            {
                tileBuilderController.DeleteAllTiles();
                tileBuilderController.CreateTile(tileBuilderController.OutdoorPrefab, new(0, 0), 0);
                tileBuilderController.CreateTile(tileBuilderController.OutdoorPrefab, new(0, 1), 0);
                tileBuilderController.CreateTile(
                    tileBuilderController.WorkingPlaceFree,
                    new(1, 0),
                    0
                );
                tileBuilderController.CreateTile(tileBuilderController.WorkingPlace, new(1, 1), 0);
            }
            if (GUILayout.Button("Update All"))
            {
                tileBuilderController.TileBuilder.UpdateAllTiles();
            }
            EditorGUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            Controller tileBuilderController = serializedObject.targetObject as Controller;

            DisplayGameModeChange(tileBuilderController);
            ShowLocationBuildingButtons(tileBuilderController);

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
