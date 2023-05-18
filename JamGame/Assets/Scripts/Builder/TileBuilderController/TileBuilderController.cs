using Common;
using System;
using UnityEngine;

public class TileBuilderController : MonoBehaviour
{
    [Header("==== Info Fields ====")]
    [SerializeField] private string CurrentValidator;
    [Header("==== Require variables ====")]
    [SerializeField] private TilesPanelController tilesPanelController;
    public TileBuilder TileBuilder;

    private IValidator validator = new GameModeValidator();
    private Vector2 previousMousePosition;
    private bool mousePressed = false;

    public Result Execute(ICommand command)
    {
        Result response = validator.ValidateCommand(command);
        return response.Success ? command.Execute(this) : response;
    }
    public void ChangeGameMode(Gamemode gamemode)
    {
        validator = gamemode switch
        {
            Gamemode.godmode => new GodModeValidator(TileBuilder),
            Gamemode.building => new BuildModeValidator(TileBuilder),
            Gamemode.gameing => new GameModeValidator(),
            _ => throw new ArgumentException(),
        };
    }

    public void Update()
    {
        CurrentValidator = validator.GetType().Name;

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
    public Result CreateTile(GameObject tilePrefab)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        AddTileToSceneCommand command = new(tilePrefab, ray);
        return Execute(command);
    }
    public void DeleteTile()
    {
        GameObject destroyedTileUIPrefab = null;
        DeleteSelectedTileCommand command = new((arg) => destroyedTileUIPrefab = arg);
        Result result = Execute(command);
        if (result.Success)
        {
            _ = tilesPanelController.CreateUIElement(destroyedTileUIPrefab);
        }
    }
}