using System.Linq;
using Common;
using Level;
using Level.Room;
using Sirenix.OdinInspector;
using TileBuilder.Command;
using TileUnion;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Utils.Raycast;

namespace TileBuilder.Controller
{
    [AddComponentMenu("Scripts/TileBuilder/Controller/TileBuilder.Controller")]
    public partial class ControllerImpl : MonoBehaviour, IDragAndDropAgent
    {
        [SerializeField]
        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private TileBuilderImpl tileBuilder;

        [SerializeField]
        [RequiredIn(PrefabKind.PrefabInstanceAndNonPrefabInstance)]
        private Level.Inventory.Controller.ControllerImpl inventory;

        public UnityEvent BuiltValidatedOffice;

        private InputActions inputActions;
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
            inputActions = new();
        }

        private void OnEnable()
        {
            inputActions.Enable();
        }

        private void OnDisable()
        {
            inputActions.Disable();
        }

        public void ChangeGameMode(GameMode gameMode)
        {
            tileBuilder.ChangeGameMode(gameMode);
        }

        public void ValidateBuilding()
        {
            Result result = tileBuilder.ExecuteCommand(new ValidateBuilding());
            if (result.Success)
            {
                BuiltValidatedOffice?.Invoke();
            }
        }

        public void Hover(CoreModel coreModel)
        {
            if (inputActions.UI.RotateTile.WasPressedThisFrame())
            {
                coreModel.TileUnionModel.PlacingProperties.ApplyRotation(
                    RotationDirection.Clockwise
                );
            }

            Result<Vector2Int> matrixResult = RaycastMatrix();
            if (matrixResult.Failure)
            {
                return;
            }

            coreModel.TileUnionModel.PlacingProperties.SetPosition(matrixResult.Data);
            ShowSelectedRoom command = new(coreModel);
            _ = tileBuilder.ExecuteCommand(command);
        }

        public Result Drop(CoreModel coreModel)
        {
            Result<Vector2Int> matrixResult = RaycastMatrix();
            if (matrixResult.Success)
            {
                coreModel.TileUnionModel.PlacingProperties.SetPosition(matrixResult.Data);
                return tileBuilder.ExecuteCommand(new DropRoom(coreModel));
            }
            else
            {
                return new FailResult(matrixResult.Error);
            }
        }

        public Result<CoreModel> Borrow()
        {
            Result<Vector2Int> matrixResult = RaycastMatrix();
            if (matrixResult.Success)
            {
                BorrowRoom command = new(matrixResult.Data);
                Result result = tileBuilder.ExecuteCommand(command);
                return result.Success
                    ? new SuccessResult<CoreModel>(command.BorrowedRoom)
                    : new FailResult<CoreModel>(result.Error);
            }
            return new FailResult<CoreModel>(matrixResult.Error);
        }

        private Result<Vector2Int> RaycastMatrix()
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = mainCamera.ScreenPointToRay(mousePosition);
            if (RayCaster.PointerIsOverUI(mousePosition))
            {
                return new FailResult<Vector2Int>("UI blocked raycast.");
            }
            return tileBuilder.GridProperties.GetMatrixPosition(ray);
        }

        public void HoverLeave()
        {
            _ = tileBuilder.ExecuteCommand(new HideSelectedRoom());
        }

        public Result GrowMeetingRoomForEmployees(int employeeCount)
        {
            MeetingRoomLogics[] meetingRooms = FindObjectsOfType<MeetingRoomLogics>();
            if (meetingRooms.Count() != 1)
            {
                Debug.LogError("Invalid MeetingRoomCount");
                return new FailResult("Invalid MeetingRoomCount");
            }
            MeetingRoomLogics currentMeetingRoom = meetingRooms.First();

            if (!currentMeetingRoom.IsCanFitEmployees(employeeCount))
            {
                return new FailResult("Cannot add more employee than maximum");
            }

            GrowMeetingRoom command =
                new(
                    currentMeetingRoom,
                    currentMeetingRoom.GetGrowCountForFitEmployees(employeeCount)
                );
            Result result = tileBuilder.ExecuteCommand(command);
            if (result.Failure)
            {
                return result;
            }
            foreach (CoreModel coreModel in command.BorrowedCoreModels)
            {
                _ = inventory.Drop(coreModel);
            }

            return new SuccessResult();
        }

        public Bounds GetBuildingBounds()
        {
            return tileBuilder.Bounds;
        }

        public void LoadBuildingFromConfig(BuildingConfig buildingConfig)
        {
            _ = tileBuilder.ExecuteCommand(new RemoveAllRooms());
            foreach (TileConfig tileConfig in buildingConfig.TilePlaceConfigs)
            {
                CoreModel core = CoreModel.InstantiateCoreModel(tileConfig);
                _ = tileBuilder.ExecuteCommand(new DropRoom(core));
            }
        }
    }
}
