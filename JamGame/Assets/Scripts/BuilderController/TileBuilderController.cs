using Common;
using System.Linq;
using UnityEngine;

public class TileBuilderController : MonoBehaviour
{
    [SerializeField] TilePrefabsHandler tilePrefabHandler;
    [SerializeField] GameObject pointerPrefab;

    // Public for unity editor
    public TileBuilder tileBuilder;
    EditorModeValidator editorMode = new();
    EditorModeValidator gameMode = new();
    bool EditorMode = false;
    bool GameMode = false;

    public void Start()
    {
        tileBuilder = new TileBuilder(tilePrefabHandler, pointerPrefab, transform, editorMode);
        SetEditorMode(true);
        tileBuilder.CreateRandomTiles();
    }

    public void SetEditorMode(bool mode)
    {
        GameMode = false;
        EditorMode = mode;
        if (EditorMode)
        {
            tileBuilder.ChangeValidator(editorMode);
        }
    }
    public void SetGameMode(bool mode)
    {
        GameMode = mode;
        EditorMode = false;
        if (GameMode)
        {
            tileBuilder.ChangeValidator(gameMode);
        }
    }
    public void Update()
    {
        if (EditorMode || GameMode)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var hits = Physics.RaycastAll(ray, 150f);
                var roomHits = hits.ToList().Where(x => x.collider.GetComponent<Tile>() != null);
                if (roomHits.Count() != 0)
                    tileBuilder.SelectTile(hits.ToList().Find(x => x.collider.GetComponent<Tile>()).collider.GetComponent<Tile>());
                else
                    tileBuilder.ComletePlacing();
            }
            if (Input.GetMouseButtonDown(1))
            {
                tileBuilder.ComletePlacing();
            }
            if (tileBuilder.IsTileSelected())
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    tileBuilder.MoveSelectedTile(Direction.Up);
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    tileBuilder.MoveSelectedTile(Direction.Left);
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    tileBuilder.MoveSelectedTile(Direction.Down);
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    tileBuilder.MoveSelectedTile(Direction.Right);
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    tileBuilder.RotateSelectedTile();
                }
            }
        }
        if (EditorMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                tileBuilder.AddTileToScene(TileType.build);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                tileBuilder.AddTileToScene(TileType.outdoor);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                tileBuilder.AddTileToScene(TileType.window);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                tileBuilder.AddTileToScene(TileType.stairs);
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                tileBuilder.ChangeXMatrixPlacing(1);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                tileBuilder.ChangeXMatrixPlacing(-1);
            }
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                tileBuilder.DeleteSelectedTile();
            }
        }
    }
}

