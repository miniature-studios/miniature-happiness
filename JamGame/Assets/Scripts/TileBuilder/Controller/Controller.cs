using Common;
using Level.Room;
using TileBuilder.Command;
using UnityEngine;
using UnityEngine.Events;

namespace TileBuilder
{
    public enum GameMode
    {
        God,
        Build,
        Play
    }

    [AddComponentMenu("Scripts/TileBuilder.Controller")]
    public partial class Controller : MonoBehaviour
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
            Result<Vector2Int> matrixResult = tileBuilder.BuilderMatrix.GetMatrixPosition(ray);
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
            Result<Vector2Int> matrixResult = tileBuilder.BuilderMatrix.GetMatrixPosition(ray);
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
            Result<Vector2Int> matrixResult = tileBuilder.BuilderMatrix.GetMatrixPosition(ray);
            if (matrixResult.Success)
            {
                CoreModel core = null;
                BorrowRoom command = new(matrixResult.Data, (coreModel) => core = coreModel);
                Result result = model.Execute(command);
                return result.Success
                    ? new SuccessResult<CoreModel>(core)
                    : new FailResult<CoreModel>(result.Error);
            }
            return new FailResult<CoreModel>(matrixResult.Error);
        }

        public void OnHoverLeave()
        {
            _ = model.Execute(new HideSelectedRoom());
        }
    }
}
