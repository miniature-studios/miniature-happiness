using Common;
using System;
using UnityEngine;
using UnityEngine.Events;

public class TileBuilderController : MonoBehaviour
{
    [SerializeField] private TileBuilder tileBuilder;
    [SerializeField] private InventoryController inventoryController;

    private IValidator validator = new GameModeValidator();
    private Vector2 previousMousePosition;
    private bool mousePressed = false;

    public UnityEvent<RoomInventoryUI> JustAddedUI; // FIXME
    public UnityEvent BuildedValidatedOffice;

    private void Awake()
    {
        inventoryController.TryPlace += TryPlace;
    }

    public Result Execute(ICommand command)
    {
        Result response = validator.ValidateCommand(command);
        return response.Success ? command.Execute(tileBuilder) : response;
    }

    public void ChangeGameMode(Gamemode gamemode)
    {
        validator = gamemode switch
        {
            Gamemode.God => new GodModeValidator(tileBuilder),
            Gamemode.Build => new BuildModeValidator(tileBuilder),
            Gamemode.Play => new GameModeValidator(),
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
                Result response = Execute(new SelectTileCommand(ray));
                if (response.Failure)
                {
                    _ = Execute(new CompletePlacingCommand());
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            mousePressed = false;
            _ = Execute(new CompletePlacingCommand());
        }

        if (mouseDelta.magnitude > 0 && mousePressed && !isOverUI)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            MoveSelectedTileCommand command = new(
                ray,
                tileBuilder.BuilderMatrix,
                tileBuilder.SelectedTile == null ? Vector2Int.zero : tileBuilder.SelectedTile.CenterPosition
                );
            _ = Execute(command);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _ = Execute(new RotateSelectedTileCommand(RotationDirection.Clockwise));
        }
    }

    public void PointerOverView(bool over)
    {
        if (over)
        {
            RoomInventoryUI destroyed_tile_ui_prefab = null;
            DeleteSelectedTileCommand command = new((arg) => destroyed_tile_ui_prefab = arg);
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
        AddTileToSceneCommand command = new(room_inventory_ui.TileUnion, ray);
        return Execute(command);
    }

    public void ValidateBuilding()
    {
        Result result = Execute(new ValidateBuildingCommand());
        if (result.Success)
        {
            BuildedValidatedOffice?.Invoke();
        }
    }
}