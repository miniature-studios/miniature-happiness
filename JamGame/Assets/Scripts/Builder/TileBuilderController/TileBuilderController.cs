using Common;
using UnityEngine;

public class TileBuilderController : MonoBehaviour
{
    [SerializeField] TileBuilder tileBuilder;
    [SerializeField] public GameObject TileToCreatePrefab;
    [SerializeField] public Transform UIHandler;
    Vector2 previousMousePosition;
    bool mousePressed = false;
    bool mouseOverUI = false;
    bool mouseUIClicked = false;
    TileUI uITileClicked = null;
    public void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseDelta = mousePosition - previousMousePosition;
        previousMousePosition = mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            mousePressed = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var command = new SelectTileCommand(tileBuilder, ray);
            var response = tileBuilder.Execute(command);
            if(response.Failure)
            {
                _ = tileBuilder.Execute(new CompletePlacingCommand(tileBuilder));
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if(mousePressed && mouseOverUI)
            {
                DeleteTile();
            }
            mousePressed = false;
            mouseUIClicked = false;
            _ = tileBuilder.Execute(new CompletePlacingCommand(tileBuilder));
        }

        if(mouseDelta.magnitude > 0 && mousePressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var command = new MoveSelectedTileToRayCommand(tileBuilder, ray);
            _ = tileBuilder.Execute(command);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            _ = tileBuilder.Execute(new RotateSelectedTileCommand(tileBuilder, Direction.Right));
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
            var result = CreateTile(uITileClicked.TileUnionPrefab);
            if (result.Success)
            {
                uITileClicked.TakeOne();
            }
        }
        mouseOverUI = false;
    }

    public void MouseUIClick(TileUI uITile)
    {
        mouseUIClicked = true;
        uITileClicked = uITile;
    }

    public void DeleteTile()
    {
        GameObject destroyedTileUIPrefab = null;
        var command = new DeleteSelectedTileCommand(tileBuilder, (arg) => destroyedTileUIPrefab = arg);
        var response = tileBuilder.Execute(command);
        if (response.Success)
        {
            _ = CreateUIElement(destroyedTileUIPrefab);
        }
    }

    public Result CreateTile(GameObject tilePrefab)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var command = new AddTileToSceneCommand(tileBuilder, tilePrefab, ray);
        return tileBuilder.Execute(command);
    }

    public TileUI CreateUIElement(GameObject UIPrefab)
    {
        var UiElement = Instantiate(UIPrefab, UIHandler);
        var ansver = UiElement.GetComponent<TileUI>().Init(MouseUIClick);
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

