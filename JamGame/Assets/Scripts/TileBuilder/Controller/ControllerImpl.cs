using Common;
using Level;
using Level.Room;
using TileBuilder.Command;
using UnityEngine;
using UnityEngine.Events;

namespace TileBuilder.Controller
{
    public enum GameMode
    {
        God,
        Build,
        Play
    }

    [AddComponentMenu("Scripts/TileBuilder.Controller.ControllerImpl")]
    public partial class ControllerImpl : MonoBehaviour, IDragAndDropAgent
    {
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        [SerializeField]
        private Model model;

        public UnityEvent BuiltValidatedOffice;

        public void ChangeGameMode(GameMode gameMode)
        {
            model.ChangeGameMode(gameMode);
        }

        public void ValidateBuilding()
        {
            Result result = model.Execute(new ValidateBuilding());
            if (result.Success)
            {
                BuiltValidatedOffice?.Invoke();
            }
        }

        public void Hover(CoreModel coreModel)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                coreModel.TileUnionModel.PlacingProperties.ApplyRotation(
                    RotationDirection.Clockwise
                );
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Result<Vector2Int> matrixResult = tileBuilder.GridProperties.GetMatrixPosition(ray);
            if (matrixResult.Failure)
            {
                return;
            }

            coreModel.TileUnionModel.PlacingProperties.SetPosition(matrixResult.Data);
            ShowSelectedRoom command = new(coreModel);
            _ = model.Execute(command);
        }

        public Result Drop(CoreModel coreModel)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Result<Vector2Int> matrixResult = tileBuilder.GridProperties.GetMatrixPosition(ray);
            if (matrixResult.Success)
            {
                coreModel.TileUnionModel.PlacingProperties.SetPosition(matrixResult.Data);
                return model.Execute(new DropRoom(coreModel));
            }
            else
            {
                return new FailResult(matrixResult.Error);
            }
        }

        public Result<CoreModel> Borrow()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Result<Vector2Int> matrixResult = tileBuilder.GridProperties.GetMatrixPosition(ray);
            if (matrixResult.Success)
            {
                BorrowRoom command = new(matrixResult.Data);
                Result result = model.Execute(command);
                return result.Success
                    ? new SuccessResult<CoreModel>(command.BorrowedRoom)
                    : new FailResult<CoreModel>(result.Error);
            }
            return new FailResult<CoreModel>(matrixResult.Error);
        }

        public void HoverLeave()
        {
            _ = model.Execute(new HideSelectedRoom());
        }
    }
}
