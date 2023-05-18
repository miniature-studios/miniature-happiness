using Common;
using System;
using UnityEngine;

public class TileBuilderController : MonoBehaviour
{
    [Header("==== For tests ====")]
    [SerializeField] private GameObject testUI;

    [Header("==== Require variables ====")]
    [SerializeField] private Transform uiHandler;
    public TileBuilder TileBuilder;
    public Gamemode GameMode;

    private IValidator validator = new GameModeValidator();
    private Vector2 previousMousePosition;
    private bool mousePressed = false;
    private bool mouseOverUI = false;
    private bool mouseUIClicked = false;
    private TileUI uiTileClicked = null;

    public void Start()
    {
        _ = CreateUIElement(testUI);
        ChangeGameMode(Gamemode.building);
    }
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

    public void SelectTileOnMousePosition()
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
    public void DeselectTile()
    {
        if (mousePressed && mouseOverUI)
        {
            DeleteTile();
        }
        mousePressed = false;
        mouseUIClicked = false;
        _ = Execute(new CompletePlacingCommand());
    }
    public void MoveSelectedTileToMousePisition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        MoveSelectedTileToRayCommand command = new(ray);
        _ = Execute(command);
    }
    public void RotateSelectedTile()
    {
        _ = Execute(new RotateSelectedTileCommand(Direction.Right));
    }

    public void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseDelta = mousePosition - previousMousePosition;
        previousMousePosition = mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            SelectTileOnMousePosition();
        }

        if (Input.GetMouseButtonUp(0))
        {
            DeselectTile();
        }

        if (mouseDelta.magnitude > 0 && mousePressed)
        {
            MoveSelectedTileToMousePisition();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateSelectedTile();
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            DeleteTile();
        }
    }

    public void MouseUIEnter()
    {
        mouseOverUI = true;
    }

    public void MouseUIExit()
    {
        if (mouseUIClicked)
        {
            Result result = CreateTile(uiTileClicked.TileUnionPrefab);
            if (result.Success)
            {
                uiTileClicked.TakeOne();
            }
        }
        mouseOverUI = false;
    }

    public void MouseUIClick(TileUI uITile)
    {
        mouseUIClicked = true;
        uiTileClicked = uITile;
    }

    public void DeleteTile()
    {
        GameObject destroyedTileUIPrefab = null;
        DeleteSelectedTileCommand command = new((arg) => destroyedTileUIPrefab = arg);
        Result result = Execute(command);
        if (result.Success)
        {
            _ = CreateUIElement(destroyedTileUIPrefab);
        }
    }

    public Result CreateTile(GameObject tilePrefab)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        AddTileToSceneCommand command = new(tilePrefab, ray);
        return Execute(command);
    }

    public TileUI CreateUIElement(GameObject UIPrefab)
    {
        GameObject UiElement = Instantiate(UIPrefab, uiHandler);
        TileUI.InitAnsver ansver = UiElement.GetComponent<TileUI>().Init(MouseUIClick);
        if (ansver.Merged)
        {
            Destroy(UiElement);
            return ansver.MergedTo;
        }
        else
        {
            return UiElement.GetComponent<TileUI>();
        }
    }

}