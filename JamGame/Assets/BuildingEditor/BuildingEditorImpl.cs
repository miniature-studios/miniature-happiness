using System;
using System.Collections.Generic;
using System.Linq;
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
    internal class BuildingEditorImpl : MonoBehaviour
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

        [SerializeField]
        private int roomsAddCount = 400;

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
        private TMP_InputField defaultBuildingSizeInput;

        [Space]
        [SerializeField]
        [Pickle(typeof(BuildingConfig), LookupType = ObjectProviderType.Assets)]
        private BuildingConfig buildingConfigToLoad;

        [Required]
        [SerializeField]
        private TMP_InputField buildingConfigNameInput;

        [SerializeField]
        private string baseSavePath = "Assets/ScriptableObjects/Building Configs";

        private void Start()
        {
            inventoryView.ShowInventory();
            inventoryController.AddRoomsFromAssets(roomsAddCount);
            coreModelByLabels = coreModelByRoomLabels.ToDictionary(
                x => x.RoomTileLabel,
                x => x.CoreModel
            );
        }

        [Button]
        private void LoadBuildingFromConfig()
        {
            tileBuilderController.LoadBuildingFromConfig(buildingConfigToLoad);
        }

        [Button]
        public void SaveBuildingIntoConfigFromLabel()
        {
            CreateBuildingConfig command = new();
            Result result = tileBuilder.ExecuteCommand(command);
            if (result.Success)
            {
                BuildingConfig config = command.BuildingConfig;
                string localPath = baseSavePath + "/" + buildingConfigNameInput.text + ".asset";
                localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
                AssetDatabase.CreateAsset(config, localPath);
                EditorGUIUtility.PingObject(config);
            }
            result.LogResult("CreateBuildingConfig");
        }

        [Button]
        public void CreateDefaultBuildingFromLabel()
        {
            Vector2Int size = new();
            string[] strings = defaultBuildingSizeInput.text.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries
            );
            size.x = int.Parse(strings[0]);
            size.y = int.Parse(strings[1]);
            CreateDefaultBuilding(size);
        }

        [Button]
        private void DeleteAllTiles()
        {
            Result result = tileBuilder.ExecuteCommand(new RemoveAllRooms());
            result.LogResult("RemoveAllRooms");
        }

        [Button]
        private void CreateTile(CoreModel coreModel, Vector2Int position, int rotation = 0)
        {
            TileConfig config = new(coreModel.Uid, position, rotation);
            CoreModel newCoreModel = CoreModel.InstantiateCoreModel(config);
            DropRoom command = new(newCoreModel);
            Result result = tileBuilder.ExecuteCommand(command);
            result.LogResult($"DropRoom pos: {position} rot: {rotation}");
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
