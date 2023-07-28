using Common;
using System;
using TileUnion;
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

    public class SelectedTileCover
    {
        private TileUnionImpl selectedTile = null;
        public TileUnionImpl Value
        {
            get => selectedTile;
            set => selectedTile = value;
        }
    }

    [AddComponentMenu("TileBuilder.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        [SerializeField, InspectorReadOnly]
        private SelectedTileCover selectedTile = new();

        [SerializeField]
        private Level.Inventory.Controller inventoryController;

        private Validator.IValidator validator = new Validator.GameMode();
        private Vector2 previousMousePosition;
        private bool mousePressed = false;

        public UnityEvent<Level.Inventory.Room.Model> JustAddedUI;
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
                GameMode.God => new Validator.GodMode(tileBuilder, selectedTile),
                GameMode.Build => new Validator.BuildMode(tileBuilder, selectedTile),
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
                    Result result = Execute(new Command.CompletePlacing(selectedTile));
                    if (result.Success)
                    {
                        Result response = Execute(new Command.SelectTile(ray, selectedTile));
                        if (response.Failure)
                        {
                            _ = Execute(new Command.CompletePlacing(selectedTile));
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                mousePressed = false;
                _ = Execute(new Command.CompletePlacing(selectedTile));
            }

            if (mouseDelta.magnitude > 0 && mousePressed && !isOverUI)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Command.MoveSelectedTile command =
                    new(
                        ray,
                        tileBuilder.BuilderMatrix,
                        selectedTile.Value == null
                            ? Vector2Int.zero
                            : selectedTile.Value.CenterPosition,
                        selectedTile
                    );
                _ = Execute(command);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                _ = Execute(
                    new Command.RotateSelectedTile(RotationDirection.Clockwise, selectedTile)
                );
            }
        }

        public void PointerOverView(bool over)
        {
            if (over)
            {
                Level.Inventory.Room.Model destroyed_tile = null;
                Command.DeleteSelectedTile command =
                    new((arg) => destroyed_tile = arg, selectedTile);
                Result result = Execute(command);
                if (result.Success)
                {
                    inventoryController.AddNewRoom(destroyed_tile);
                    JustAddedUI?.Invoke(destroyed_tile);
                }
            }
        }

        private Result TryPlace(Level.Inventory.Room.Model room)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Command.AddTileToScene command = new(room.TileUnion, ray, selectedTile);
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
