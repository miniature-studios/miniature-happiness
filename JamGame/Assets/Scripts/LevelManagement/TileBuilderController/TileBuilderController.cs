using Common;
using System;
using UnityEngine;

public class TileBuilderController : MonoBehaviour
{
    [Header("==== Info Fields ====")]
    [SerializeField] private string currentValidator;

    [Header("==== Require variables ====")]
    [SerializeField] private CustomButton buttonCompleteMeeting;

    public TilesPanelController TilesPanelController;
    public TileBuilder TileBuilder;
    public Action СompleteMeetingEvent;

    private IValidator validator = new GameModeValidator();
    private Vector2 previousMousePosition;
    private bool mousePressed = false;

    public UIHider ButtonCompleteMeetingUIHider => buttonCompleteMeeting.UIHider;

    public Result Execute(ICommand command)
    {
        Result response = validator.ValidateCommand(command);
        return response.Success ? command.Execute(this) : response;
    }

    public void ChangeGameMode(Gamemode gamemode)
    {
        validator = gamemode switch
        {
            Gamemode.GodMode => new GodModeValidator(TileBuilder),
            Gamemode.Building => new BuildModeValidator(TileBuilder),
            Gamemode.Gameing => new GameModeValidator(),
            _ => throw new ArgumentException(),
        };
    }

    private void Update()
    {
        currentValidator = validator.GetType().Name;

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
            TilesPanelController.DeselectTile(mousePressed);
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
        TileUI destroyed_tile_ui_prefab = null;
        DeleteSelectedTileCommand command = new((arg) => destroyed_tile_ui_prefab = arg);
        Result result = Execute(command);
        if (result.Success)
        {
            _ = TilesPanelController.CreateUIElement(destroyed_tile_ui_prefab);
        }
    }

    public void TryCompleteMeeting()
    {
        if (TileBuilder.CheckBuildingForConsistance())
        {
            buttonCompleteMeeting.UIHider.SetState(UIElementState.Hidden);
            TilesPanelController.gameObject.SetActive(false);
            СompleteMeetingEvent?.Invoke();
        }
        else
        {
            // TODO Show no consistence
        }
    }
}