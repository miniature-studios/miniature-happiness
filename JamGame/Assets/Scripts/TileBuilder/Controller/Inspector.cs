#if UNITY_EDITOR
using Level.Room;
using Pickle;
using TileBuilder.Command;
using UnityEditor;
using UnityEngine;

namespace TileBuilder.Controller
{
    public partial class ControllerImpl
    {
        public CoreModel FreeSpace => tileBuilder.FreeSpace;

        private void Start()
        {
            if (LoadConfigFromStart)
            {
                //LoadBuildingFromConfig(BuildingConfig);
            }
        }

        public void DeleteAllTiles()
        {
            _ = tileBuilder.ExecuteCommand(new RemoveAllRooms());
        }

        public void CreateTile(CoreModel coreModel, Vector2Int position, int rotation)
        {
            CoreModel newCoreModel = CoreModel.InstantiateCoreModel(
                new TileConfig(coreModel.Uid, position, rotation)
            );
            _ = tileBuilder.ExecuteCommand(new DropRoom(newCoreModel));
        }

        public BuildingConfig SaveBuildingIntoConfig()
        {
            return tileBuilder.SaveBuildingIntoConfig();
        }

        [HideInInspector]
        public BuildingConfig BuildingConfig;

        [HideInInspector]
        public bool LoadConfigFromStart = true;

        [HideInInspector]
        public string SavingConfigName = "Sample building";

        [HideInInspector]
        public GameMode GameModeToChange = GameMode.God;

        [HideInInspector]
        public int SquareSideLength = 30;

        [HideInInspector]
        public int NeededEmployeeCount = 0;

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

        [Pickle(LookupType = ObjectProviderType.Assets)]
        public CoreModel BossOffice;
    }

    [CustomEditor(typeof(ControllerImpl))]
    public class Inspector : Editor
    {
        public void DisplayGameModeChange(ControllerImpl controller)
        {
            _ = EditorGUILayout.BeginHorizontal();
            controller.GameModeToChange = (GameMode)
                EditorGUILayout.EnumPopup(controller.GameModeToChange);
            EditorGUILayout.EndHorizontal();
            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Change game mode"))
            {
                controller.ChangeGameMode(controller.GameModeToChange);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowLocationBuildingButtons(ControllerImpl controller)
        {
            _ = EditorGUILayout.BeginHorizontal();
            controller.SquareSideLength = EditorGUILayout.IntField(
                "Square side length: ",
                controller.SquareSideLength
            );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Create random building"))
            {
                int x = 0;
                int y = 0;
                controller.DeleteAllTiles();
                for (int i = 0; i < controller.SquareSideLength * controller.SquareSideLength; i++)
                {
                    float value = Random.value * 100;
                    if (value < 50)
                    {
                        controller.CreateTile(controller.FreeSpace, new(x, y), 0);
                    }
                    else if (value is > 50 and < 65)
                    {
                        controller.CreateTile(controller.StairsPrefab, new(x, y), 0);
                    }
                    else if (value is > 65 and < 80)
                    {
                        controller.CreateTile(controller.WindowPrefab, new(x, y), 0);
                    }
                    else if (value > 80)
                    {
                        controller.CreateTile(controller.OutdoorPrefab, new(x, y), 0);
                    }
                    y++;
                    if (y >= controller.SquareSideLength)
                    {
                        y = 0;
                        x++;
                    }
                }
            }
            if (GUILayout.Button("Create normal building"))
            {
                controller.DeleteAllTiles();
                for (int i = 0; i < 9; i++)
                {
                    controller.CreateTile(controller.OutdoorPrefab, new(0, i), 0);
                }

                for (int i = 0; i < 8; i++)
                {
                    if (i == 1)
                    {
                        controller.CreateTile(controller.StairsPrefab, new(i + 1, 0), 0);
                    }
                    else
                    {
                        controller.CreateTile(controller.OutdoorPrefab, new(i + 1, 0), 0);
                    }

                    for (int j = 0; j < 7; j++)
                    {
                        if (j == 2)
                        {
                            controller.CreateTile(controller.CorridorPrefab, new(i + 1, j + 1), 0);
                        }
                        else if (j == 3)
                        {
                            controller.CreateTile(controller.WorkingPlace, new(i + 1, j + 1), 1);
                        }
                        else if (j == 4)
                        {
                            controller.CreateTile(
                                controller.WorkingPlaceFree,
                                new(i + 1, j + 1),
                                0
                            );
                        }
                        else
                        {
                            controller.CreateTile(controller.FreeSpace, new(i + 1, j + 1), 0);
                        }
                    }
                    controller.CreateTile(controller.OutdoorPrefab, new(i + 1, 8), 0);
                }
                for (int i = 0; i < 9; i++)
                {
                    controller.CreateTile(controller.OutdoorPrefab, new(9, i), 0);
                }
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear Scene"))
            {
                controller.DeleteAllTiles();
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Fill with boss"))
            {
                int x = 0;
                int y = 0;
                controller.DeleteAllTiles();
                for (int i = 0; i < controller.SquareSideLength * controller.SquareSideLength; i++)
                {
                    controller.CreateTile(controller.BossOffice, new(x, y), 0);
                    y++;
                    if (y >= controller.SquareSideLength)
                    {
                        y = 0;
                        x++;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Add 4 Tiles"))
            {
                controller.DeleteAllTiles();
                controller.CreateTile(controller.OutdoorPrefab, new(0, 0), 0);
                controller.CreateTile(controller.OutdoorPrefab, new(0, 1), 0);
                controller.CreateTile(controller.WorkingPlaceFree, new(1, 0), 0);
                controller.CreateTile(controller.WorkingPlace, new(1, 1), 0);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            controller.NeededEmployeeCount = EditorGUILayout.IntField(
                "Employee count to extend: ",
                controller.NeededEmployeeCount
            );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Grow Meeting room"))
            {
                Common.Result result = controller.GrowMeetingRoomForEmployees(
                    controller.NeededEmployeeCount
                );
                if (result.Failure)
                {
                    Debug.Log(result.Error);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void ShowSaveLoading(ControllerImpl tileBuilderController)
        {
            _ = EditorGUILayout.BeginHorizontal();
            tileBuilderController.BuildingConfig = (BuildingConfig)
                EditorGUILayout.ObjectField(
                    "Loading prefab: ",
                    tileBuilderController.BuildingConfig,
                    typeof(BuildingConfig),
                    false
                );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            tileBuilderController.SavingConfigName = EditorGUILayout.TextField(
                "Saving config name: ",
                tileBuilderController.SavingConfigName
            );
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Load building from config."))
            {
                tileBuilderController.LoadBuildingFromConfig(tileBuilderController.BuildingConfig);
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save building into config."))
            {
                BuildingConfig config = tileBuilderController.SaveBuildingIntoConfig();
                string localPath =
                    "Assets/ScriptableObjects/Building Configs/"
                    + tileBuilderController.SavingConfigName
                    + ".asset";
                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                AssetDatabase.CreateAsset(config, localPath);
                Debug.Log("Asset was saved successfully");
            }
            EditorGUILayout.EndHorizontal();

            _ = EditorGUILayout.BeginHorizontal();
            tileBuilderController.LoadConfigFromStart = EditorGUILayout.Toggle(
                "Load from config on start?",
                tileBuilderController.LoadConfigFromStart
            );
            EditorGUILayout.EndHorizontal();
        }

        public override void OnInspectorGUI()
        {
            ControllerImpl tileBuilderController = serializedObject.targetObject as ControllerImpl;

            DisplayGameModeChange(tileBuilderController);
            ShowLocationBuildingButtons(tileBuilderController);
            ShowSaveLoading(tileBuilderController);

            _ = DrawDefaultInspector();

            _ = serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
