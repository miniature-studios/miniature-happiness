#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using CameraController;
using Common;
using Level.Room;
using Pickle;
using Sirenix.OdinInspector;
using TileBuilder;
using TileBuilder.Command;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace BuildingEditor
{
    [AddComponentMenu("Scripts/BuildingEditor/BuildingEditor")]
    internal class BuildingEditorImpl : MonoBehaviour, IBoundsSource
    {
        [Serializable]
        private struct CoreModelByRoomLabel
        {
            public RoomTileLabel RoomTileLabel;

            [Pickle(typeof(CoreModel), LookupType = ObjectProviderType.Assets)]
            public CoreModel CoreModel;
        }

        [Title("Inventory")]
        [Required]
        [SerializeField]
        private Level.Inventory.Controller.ControllerImpl inventoryController;

        [Required]
        [SerializeField]
        private Level.Inventory.View inventoryView;

        private const int INITIAL_ROOM_COUNT = 500;

        [Title("Tile Builder")]
        [Required]
        [SerializeField]
        private Controller tileBuilderController;

        [Required]
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        [SerializeField]
        private List<CoreModelByRoomLabel> coreModelByRoomLabels = new();
        private Dictionary<RoomTileLabel, CoreModel> coreModelByLabels;

        [Space]
        [Required]
        [SerializeField]
        private TMP_InputField buildingSizeInputX;

        [Required]
        [SerializeField]
        private TMP_InputField buildingSizeInputY;

        [Space]
        [SerializeField]
        [Pickle(typeof(BuildingConfig), LookupType = ObjectProviderType.Assets)]
        private BuildingConfig buildingConfigToLoad;

        [Required]
        [SerializeField]
        private TMP_InputField buildingConfigNameInput;

        [Required]
        [SerializeField]
        private TMP_Dropdown buildingModeDropdown;
        private List<GameMode> gameModes;

        [SerializeField]
        private string baseSavePath = "Assets/ScriptableObjects/Building Configs";

        public Bounds Bounds => new(Vector3.zero, Vector3.positiveInfinity);

        private void Start()
        {
            buildingModeDropdown.ClearOptions();
            gameModes = Enum.GetValues(typeof(GameMode)).Cast<GameMode>().ToList();
            buildingModeDropdown.AddOptions(gameModes.Select(x => x.ToString()).ToList());
            buildingModeDropdown.value = 0;

            inventoryView.ShowInventory();
            inventoryController.AddRoomsFromAssets(INITIAL_ROOM_COUNT);
            coreModelByLabels = coreModelByRoomLabels.ToDictionary(
                x => x.RoomTileLabel,
                x => x.CoreModel
            );
        }

        [Button]
        public void BuildingModeChanged(int value)
        {
            tileBuilder.ChangeGameMode(gameModes[value]);
        }

        [Button]
        private void LoadBuildingFromConfig()
        {
            tileBuilderController.LoadBuildingFromConfig(buildingConfigToLoad);
        }

        [Button]
        public void SaveBuildingIntoConfigFromLabel()
        {
            BuildingConfig config = tileBuilder.CreateBuildingConfig();
            string localPath = baseSavePath + "/" + buildingConfigNameInput.text + ".asset";
            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            AssetDatabase.CreateAsset(config, localPath);
            EditorGUIUtility.PingObject(config);
        }

        [Button]
        public void CreateDefaultBuildingFromLabel()
        {
            Vector2Int size =
                new(int.Parse(buildingSizeInputX.text), int.Parse(buildingSizeInputY.text));
            CreateDefaultBuilding(size);
        }

        [Button]
        private void DeleteAllTiles()
        {
            _ = tileBuilder.ExecuteCommand(new RemoveAllRooms());
        }

        [Button]
        private void CreateTile(CoreModel coreModel, Vector2Int position, int rotation = 0)
        {
            TileConfig config = new(coreModel.Uid, position, rotation);
            CoreModel newCoreModel = CoreModel.InstantiateCoreModel(config);
            DropRoom command = new(newCoreModel);
            _ = tileBuilder.ExecuteCommand(command);
        }

        [Button]
        private void CreateTile(RoomTileLabel roomTileLabel, Vector2Int position, int rotation = 0)
        {
            CreateTile(coreModelByLabels[roomTileLabel], position, rotation);
        }

        [Button]
        private void CreateDefaultBuilding(Vector2Int size)
        {
            DeleteAllTiles();

            IEnumerable<int> xLine = Enumerable.Range(0, size.x);
            IEnumerable<int> yLine = Enumerable.Range(0, size.y);

            IEnumerable<Vector2Int> insidePositions = xLine.SelectMany(x =>
                yLine.Select(y => new Vector2Int(x, y))
            );
            foreach (Vector2Int position in insidePositions)
            {
                CreateTile(RoomTileLabel.FreeSpace, position);
            }

            List<Vector2Int> outsidePositions = new();
            outsidePositions.AddRange(xLine.Select(x => new Vector2Int(x, -1)));
            outsidePositions.AddRange(xLine.Select(x => new Vector2Int(x, size.y)));
            outsidePositions.AddRange(yLine.Select(y => new Vector2Int(-1, y)));
            outsidePositions.AddRange(yLine.Select(y => new Vector2Int(size.x, y)));
            outsidePositions.AddRange(
                new List<Vector2Int>() { new(-1, -1), size, new(-1, size.y), new(size.x, -1) }
            );

            foreach (Vector2Int position in outsidePositions)
            {
                CreateTile(RoomTileLabel.Outside, position);
            }
        }
    }
}
#endif
