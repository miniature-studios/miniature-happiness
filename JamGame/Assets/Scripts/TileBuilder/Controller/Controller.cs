using Common;
using Level.Room;
using System;
using System.Collections.Generic;
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

    public class SelectedTileWrapper
    {
        private TileUnionImpl selectedTile = null;
        public TileUnionImpl Value
        {
            get => selectedTile;
            set => selectedTile = value;
        }
    }

    [AddComponentMenu("Scripts/TileBuilder.Controller")]
    public class Controller : MonoBehaviour
    {
        [SerializeField]
        private TileBuilderImpl tileBuilder;

        [SerializeField]
        private List<CoreModel> models;

        // public for inspector
        [InspectorReadOnly]
        public SelectedTileWrapper SelectedTileWrapper = new();

        [SerializeField]
        private Level.Inventory.Controller inventoryController;

        private Validator.IValidator validator = new Validator.GameMode();
        private Vector2 previousMousePosition;
        private bool mousePressed = false;

        public UnityEvent<CoreModel> JustAddedUI;
        public UnityEvent BuiltValidatedOffice;

        private void Awake()
        {
            inventoryController.TryPlace += TryPlace;
        }

        public Result Execute(Command.ICommand command)
        {
            Result response = validator.ValidateCommand(command);
            return response.Success ? command.Execute(tileBuilder) : response;
        }

        public void ChangeGameMode(GameMode game_mode)
        {
            validator = game_mode switch
            {
                GameMode.God => new Validator.GodMode(tileBuilder, SelectedTileWrapper),
                GameMode.Build => new Validator.BuildMode(tileBuilder, SelectedTileWrapper),
                GameMode.Play => new Validator.GameMode(),
                _ => throw new ArgumentException(),
            };
        }

        private void Update()
        {
            Vector2 mousePosition = Input.mousePosition;
            Vector2 mouseDelta = mousePosition - previousMousePosition;
            previousMousePosition = mousePosition;

            bool isOverUI = RayCastUtilities.PointerIsOverUI(mousePosition);

            if (Input.GetMouseButtonDown(0))
            {
                mousePressed = true;
                if (!isOverUI)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Result result = Execute(new Command.CompletePlacing(SelectedTileWrapper));
                    if (result.Success)
                    {
                        Result response = Execute(new Command.SelectTile(ray, SelectedTileWrapper));
                        if (response.Failure)
                        {
                            _ = Execute(new Command.CompletePlacing(SelectedTileWrapper));
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                mousePressed = false;
                _ = Execute(new Command.CompletePlacing(SelectedTileWrapper));
            }

            if (mouseDelta.magnitude > 0 && mousePressed && !isOverUI)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Command.MoveSelectedTile command =
                    new(
                        ray,
                        tileBuilder.BuilderMatrix,
                        SelectedTileWrapper.Value == null
                            ? Vector2Int.zero
                            : SelectedTileWrapper.Value.CenterPosition,
                        SelectedTileWrapper
                    );
                _ = Execute(command);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                _ = Execute(
                    new Command.RotateSelectedTile(RotationDirection.Clockwise, SelectedTileWrapper)
                );
            }
        }

        public void PointerOverView(bool over)
        {
            if (over)
            {
                Level.Room.CoreModel destroyed_tile = null;
                Command.DeleteSelectedTile command =
                    new((arg) => destroyed_tile = arg, SelectedTileWrapper);
                Result result = Execute(command);
                if (result.Success)
                {
                    inventoryController.AddNewRoom(destroyed_tile);
                    JustAddedUI?.Invoke(destroyed_tile);
                }
            }
        }

        private Result TryPlace(Level.Room.CoreModel room)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Command.AddTileToScene command = new(room.TileUnionPrefab, ray, SelectedTileWrapper);
            return Execute(command);
        }

        public void ValidateBuilding()
        {
            Result result = Execute(new Command.ValidateBuilding());
            if (result.Success)
            {
                BuiltValidatedOffice?.Invoke();
            }
        }
    }
}
