using Common;
using UnityEngine;

public class TileBuilderController : MonoBehaviour
{
    [SerializeField] public TileBuilder tileBuilder;
    [SerializeField] bool EditorMode = true;
    public void Update()
    {
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

            if (Input.GetMouseButtonDown(0))
            {
                tileBuilder.SelectTileByClick();
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
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    tileBuilder.DeleteSelectedTile();
                }
            }
        }
    }
}

