using Common;
using UnityEngine;

public class TileBuilderController : MonoBehaviour
{
    [SerializeField] TileBuilder tileBuilder;
    [SerializeField] public GameObject TileToCreatePrefab;
    [SerializeField] public GameObject TileUIPrefab;
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
            Debug.Log(response.Message);
            if(!response.Accepted)
            {
                var secondresponse = tileBuilder.Execute(new CompletePlacingCommand(tileBuilder));
                Debug.Log(secondresponse.Message);
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
            var response = tileBuilder.Execute(new CompletePlacingCommand(tileBuilder));
            Debug.Log(response.Message);
        }

        if(mouseDelta.magnitude > 0 && mousePressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var command = new MoveSelectedTileToRayCommand(tileBuilder, ray);
            var response = tileBuilder.Execute(command);
            Debug.Log(response.Message);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            var response = tileBuilder.Execute(new RotateSelectedTileCommand(tileBuilder, Direction.Right));
            Debug.Log(response.Message);
        }
        if(Input.mouseScrollDelta.y > 0)
        {
            var response = tileBuilder.Execute(new RotateSelectedTileCommand(tileBuilder, Direction.Right));
            Debug.Log(response.Message);
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            var response = tileBuilder.Execute(new RotateSelectedTileCommand(tileBuilder, Direction.Left));
            Debug.Log(response.Message);
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
            var response = CreateTile(uITileClicked.TileUnionPrefab);
            if (response.Accepted)
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

    public void DeleteTile(bool stillselected = false)
    {
        GameObject destroyedTileUIPrefab = null;
        var command = new DeleteSelectedTileCommand(tileBuilder, (arg) => destroyedTileUIPrefab = arg);
        var response = tileBuilder.Execute(command);
        Debug.Log(response.Message);
        if (response.Accepted)
        {
            if (stillselected)
                uITileClicked = CreateUIElement(destroyedTileUIPrefab);
            else
                _ = CreateUIElement(destroyedTileUIPrefab);
        }
        
    }
    public Response CreateTile(GameObject tilePrefab)
    {
        var command = new AddTileToSceneCommand(tileBuilder, tilePrefab);
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

