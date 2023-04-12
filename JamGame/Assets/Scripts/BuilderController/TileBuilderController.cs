using Common;
using System.Linq;
using UnityEngine;

public class TileBuilderController : MonoBehaviour
{
    [SerializeField] BuilderValidator builderValidator;
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 150f);
            var roomHits = hits.ToList().Where(x => x.collider.GetComponent<Tile>() != null);
            if (roomHits.Count() != 0)
                builderValidator.SelectTile(hits.ToList().Find(x => x.collider.GetComponent<Tile>()).collider.GetComponent<Tile>());
            else
                builderValidator.ComletePlacing();
        }
        if (Input.GetMouseButtonDown(1))
        {
            builderValidator.ComletePlacing();
        }
        if (builderValidator.IsTileSelected())
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                builderValidator.MoveSelectedTile(Direction.Up);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                builderValidator.MoveSelectedTile(Direction.Left);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                builderValidator.MoveSelectedTile(Direction.Down);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                builderValidator.MoveSelectedTile(Direction.Right);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                builderValidator.RotateSelectedTile();
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            builderValidator.ChangeXMatrixPlacing(1);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            builderValidator.ChangeXMatrixPlacing(-1);
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            builderValidator.DeleteSelectedTile();
        }
    }
}

