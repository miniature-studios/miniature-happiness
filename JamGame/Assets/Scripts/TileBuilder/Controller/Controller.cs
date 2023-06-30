using Common;
using System;
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

    [AddComponentMenu("TileBuilder.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        [SerializeField]
        private Level.Inventory.Controller inventoryController;

        private Validator.IValidator validator = new Validator.GameMode();
        private Vector2 previousMousePosition;
        private bool mousePressed = false;

        public UnityEvent<RoomInventoryUI> JustAddedUI; // FIXME
        public UnityEvent BuildedValidatedOffice;

        private void Awake()
        {
            inventoryController.TryPlace += TryPlace;
        }

        public Result Execute(Command.ICommand command)
        {
            Result response = validator.ValidateCommand(command);
            return response.Success ? command.Execute(tileBuilder) : response;
        }

        public void ChangeGameMode(GameMode gamemode)
        {
            validator = gamemode switch
            {
                GameMode.God => new Validator.GodMode(tileBuilder),
                GameMode.Build => new Validator.BuildMode(tileBuilder),
                GameMode.Play => new Validator.GameMode(),
                _ => throw new ArgumentException(),
            };
        }

        private void Update()
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 mouseDelta = mousePosition - previousMousePosition;
            previousMousePosition = mousePosition;

            bool isOverUI = RaycastUtilities.PointerIsOverUI(Input.mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                mousePressed = true;
                if (!isOverUI)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Result response = Execute(new Command.SelectTile(ray));
                    if (response.Failure)
                    {
                        _ = Execute(new Command.CompletePlacing());
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                mousePressed = false;
                _ = Execute(new Command.CompletePlacing());
            }

            if (mouseDelta.magnitude > 0 && mousePressed && !isOverUI)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Command.MoveSelectedTile command =
                    new(
                        ray,
                        tileBuilder.BuilderMatrix,
                        tileBuilder.SelectedTile == null
                            ? Vector2Int.zero
                            : tileBuilder.SelectedTile.CenterPosition
                    );
                _ = Execute(command);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                _ = Execute(new Command.RotateSelectedTile(RotationDirection.Clockwise));
            }
        }

        public void PointerOverView(bool over)
        {
            if (over)
            {
                RoomInventoryUI destroyed_tile_ui_prefab = null;
                Command.DeleteSelectedTile command = new((arg) => destroyed_tile_ui_prefab = arg);
                Result result = Execute(command);
                if (result.Success)
                {
                    inventoryController.AddNewRoom(destroyed_tile_ui_prefab);
                    JustAddedUI?.Invoke(destroyed_tile_ui_prefab);
                }
            }
        }

        private Result TryPlace(RoomInventoryUI room_inventory_ui)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Command.AddTileToScene command = new(room_inventory_ui.TileUnion, ray);
            return Execute(command);
        }

        public void ValidateBuilding()
        {
            Result result = Execute(new Command.ValidateBuilding());
            if (result.Success)
            {
                BuildedValidatedOffice?.Invoke();
            }
        }
    }
}
