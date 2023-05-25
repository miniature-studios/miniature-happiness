using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TileBuilderController : MonoBehaviour
{
    [SerializeField] private TileBuilder tileBuilder;
    [SerializeField] private InventoryUIController tilesPanelController;
    public TileBuilder TileBuilder => tileBuilder;

    private IValidator validator = new GameModeValidator();
    private Vector2 previousMousePosition;
    private bool mousePressed = false;

    public event Action BuildedValidatedOffice;

    public Result Execute(ICommand command)
    {
        Result response = validator.ValidateCommand(command);
        return response.Success ? command.Execute(this) : response;
    }

    public void ChangeGameMode(Gamemode gamemode)
    {
        validator = gamemode switch
        {
            Gamemode.GodMode => new GodModeValidator(tileBuilder),
            Gamemode.Building => new BuildModeValidator(tileBuilder),
            Gamemode.Gameing => new GameModeValidator(),
            _ => throw new ArgumentException(),
        };
    }

    private void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseDelta = mousePosition - previousMousePosition;
        previousMousePosition = mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            mousePressed = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            SelectTileCommand command = new(ray);
            Result response = Execute(command);
            if (response.Failure)
            {
                _ = Execute(new CompletePlacingCommand());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            tilesPanelController.DeselectTile(mousePressed);
            mousePressed = false;
            _ = Execute(new CompletePlacingCommand());
        }

        if (mouseDelta.magnitude > 0 && mousePressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            MoveSelectedTileToRayCommand command = new(ray);
            _ = Execute(command);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _ = Execute(new RotateSelectedTileCommand(Direction.Right));
        }
    }

    public Result CreateTile(TileUnion tile_prefab)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        AddTileToSceneCommand command = new(tile_prefab, ray);
        return Execute(command);
    }

    public void DeleteTile()
    {
        RoomInventoryUI destroyed_tile_ui_prefab = null;
        DeleteSelectedTileCommand command = new((arg) => destroyed_tile_ui_prefab = arg);
        Result result = Execute(command);
        if (result.Success)
        {
            tilesPanelController.CreateUIElement(destroyed_tile_ui_prefab);
        }
    }

    // TODO as command
    public void ValidateBuilding()
    {
        if (tileBuilder.Validate().Success)
        {
            BuildedValidatedOffice();
        }
    }

    // TODO as another class
    public struct OfficeInfo
    {
        public int InsideTilesCount;
        public IEnumerable<RoomProperties> RoomProperties;
    }

    public OfficeInfo GetOfficeInfo()
    {
        return new()
        {
            InsideTilesCount = tileBuilder.GetAllInsideListPositions().Count(),
            RoomProperties = tileBuilder
                .GetTileUnionsInPositions(tileBuilder.GetAllInsideListPositions())
                .Where(x => x.TryGetComponent(out RoomProperties roomProperties))
                .Select(x => x.GetComponent<RoomProperties>())
        };
    }
}