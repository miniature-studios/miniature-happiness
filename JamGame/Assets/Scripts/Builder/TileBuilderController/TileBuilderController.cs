using Common;
using UnityEngine;

public class TileBuilderController : MonoBehaviour
{
    [Header("==== For debugging ====")]
    [SerializeField] public GameObject CorridorUIPrefab;
    [SerializeField] public GameObject WorkingSpaceUIPrefab;
    [SerializeField] public GameObject WorkingSpaceFreeUIPrefab;
    [SerializeField] public GameObject TripleWorkingSpaceUIPrefab;

    [Header("==== Require variables ====")]
    [SerializeField] private TileBuilder TileBuilder;
    [SerializeField] public Transform UIHandler;
    private Vector2 previousMousePosition;
    private bool mousePressed = false;
    private bool mouseOverUI = false;
    private bool mouseUIClicked = false;
    private TileUI uiTileClicked = null;

    private void Start()
    {
        AddUIElements();
    }

    public void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseDelta = mousePosition - previousMousePosition;
        previousMousePosition = mousePosition;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (Input.GetMouseButtonDown(0))
        {
            mousePressed = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            SelectTileCommand command = new(ray);
            Result response = TileBuilder.Execute(command);
            if (response.Failure)
            {
                _ = TileBuilder.Execute(new CompletePlacingCommand());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (mousePressed && mouseOverUI)
            {
                DeleteTile();
            }
            mousePressed = false;
            mouseUIClicked = false;
            _ = TileBuilder.Execute(new CompletePlacingCommand());
        }

        if (mouseDelta.magnitude > 0 && mousePressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            MoveSelectedTileToRayCommand command = new(ray);
            _ = TileBuilder.Execute(command);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _ = TileBuilder.Execute(new RotateSelectedTileCommand(Direction.Right));
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
        Result response = TileBuilder.Execute(command);
        if (response.Success)
        {
            _ = CreateUIElement(destroyedTileUIPrefab);
        }
    }

    public Result CreateTile(GameObject tilePrefab)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        AddTileToSceneCommand command = new(tilePrefab, ray);
        return TileBuilder.Execute(command);
    }

    public TileUI CreateUIElement(GameObject UIPrefab)
    {
        GameObject UiElement = Instantiate(UIPrefab, UIHandler);
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

    public void AddUIElements()
    {
        _ = CreateUIElement(CorridorUIPrefab);
        _ = CreateUIElement(CorridorUIPrefab);
        _ = CreateUIElement(CorridorUIPrefab);
        _ = CreateUIElement(WorkingSpaceUIPrefab);
        _ = CreateUIElement(WorkingSpaceUIPrefab);
        _ = CreateUIElement(WorkingSpaceUIPrefab);
        _ = CreateUIElement(WorkingSpaceFreeUIPrefab);
        _ = CreateUIElement(WorkingSpaceFreeUIPrefab);
        _ = CreateUIElement(WorkingSpaceFreeUIPrefab);
        _ = CreateUIElement(TripleWorkingSpaceUIPrefab);
        _ = CreateUIElement(TripleWorkingSpaceUIPrefab);
        _ = CreateUIElement(TripleWorkingSpaceUIPrefab);
    }
}

