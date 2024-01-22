using System.Linq;
using Common;
using Level;
using Level.Room;
using TileBuilder.Command;
using TileUnion;
using UnityEngine;
using UnityEngine.Events;

namespace TileBuilder.Controller
{
    [AddComponentMenu("Scripts/TileBuilder/Controller/TileBuilder.Controller")]
    public partial class ControllerImpl : MonoBehaviour, IDragAndDropAgent
    {
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        [SerializeField]
        private Level.Inventory.Controller inventory;

        public UnityEvent BuiltValidatedOffice;

        private InputActions inputActions;

        private void Awake()
        {
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
            if (inputActions.UI.RotateTile.IsPressed())
            {
                coreModel.TileUnionModel.PlacingProperties.ApplyRotation(
                    RotationDirection.Clockwise
                );
            }

            Ray ray = Camera.main.ScreenPointToRay(
                inputActions.UI.PointPosition.ReadValue<Vector2>()
            );
            Result<Vector2Int> matrixResult = tileBuilder.GridProperties.GetMatrixPosition(ray);
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
            Ray ray = Camera.main.ScreenPointToRay(
                inputActions.UI.PointPosition.ReadValue<Vector2>()
            );
            Result<Vector2Int> matrixResult = tileBuilder.GridProperties.GetMatrixPosition(ray);
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
            Ray ray = Camera.main.ScreenPointToRay(
                inputActions.UI.PointPosition.ReadValue<Vector2>()
            );
            Result<Vector2Int> matrixResult = tileBuilder.GridProperties.GetMatrixPosition(ray);
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
    }
}
