using Common;
using System.Linq;
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
    UITile uITileClicked = null;
    public void Update()
    {
        Vector2 mousePosition = Input.mousePosition;
        Vector2 mouseDelta = mousePosition - previousMousePosition;
        previousMousePosition = mousePosition;

        if (Input.GetMouseButtonDown(0))
        {
            mousePressed = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 300f);
            var roomHits = hits.ToList().Where(x => x.collider.GetComponent<Tile>() != null);
            if (roomHits.Count() != 0)
            {
                var tile = hits.ToList().Find(x => x.collider.GetComponent<Tile>()).collider.GetComponent<Tile>();
                var command = new SelectTileCommand(tileBuilder, tile);
                var response = tileBuilder.Execute(command);
                Debug.Log(response.Message);
            }
            else
            {
                var response = tileBuilder.Execute(new ComplatePlacingCommand(tileBuilder));
                Debug.Log(response.Message);
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
            var response = tileBuilder.Execute(new ComplatePlacingCommand(tileBuilder));
            Debug.Log(response.Message);
        }

        if(mouseDelta.magnitude > 0 && mousePressed)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var command = new MoveSelectedTileToRayCommand(tileBuilder, ray);
            var response = tileBuilder.Execute(command);
            Debug.Log(response.Message);
        }

        /*
        if (Input.GetKeyDown(KeyCode.W))
        {
            var command = new MoveSelectedTileCommand(tileBuilder, Direction.Up);
            var response = tileBuilder.Execute(command);
            Debug.Log(response.Message);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            var command = new MoveSelectedTileCommand(tileBuilder, Direction.Left);
            var response = tileBuilder.Execute(command);
            Debug.Log(response.Message);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            var command = new MoveSelectedTileCommand(tileBuilder, Direction.Down);
            var response = tileBuilder.Execute(command);
            Debug.Log(response.Message);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            var command = new MoveSelectedTileCommand(tileBuilder, Direction.Right);
            var response = tileBuilder.Execute(command);
            Debug.Log(response.Message);
        }
        */

        if (Input.GetKeyDown(KeyCode.R))
        {
            var response = tileBuilder.Execute(new RotateSelectedTileCommand(tileBuilder));
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
            if(response.Accepted)
                Destroy(uITileClicked.gameObject);
        }
        mouseOverUI = false;
    }

    public void MouseUIClick(UITile uITile)
    {
        mouseUIClicked = true;
        uITileClicked = uITile;
    }

    public void DeleteTile(bool stillselected = false)
    {
        GameObject destroyedTile = null;
        var command = new DeleteSelectedTileCommand(tileBuilder, (arg) => destroyedTile = arg);
        var response = tileBuilder.Execute(command);
        Debug.Log(response.Message);
        if (response.Accepted)
        {
            if (stillselected)
                uITileClicked = AddUIElement(destroyedTile);
            else
                _ = AddUIElement(destroyedTile);
        }
        
    }

    public Response CreateTile(GameObject tilePrefab)
    {
        var command = new AddTileToSceneCommand(tileBuilder, tilePrefab);
        return tileBuilder.Execute(command);
    }
    public UITile AddUIElement(GameObject TilePrefab)
    {
        var UiElement = Instantiate(TileUIPrefab, UIHandler);
        UiElement.GetComponent<UITile>().Init(TilePrefab, MouseUIClick);
        return UiElement.GetComponent<UITile>();
    }
}

