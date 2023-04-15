using Common;
using System.Linq;
using UnityEngine;

public class TileBuilderController : MonoBehaviour
{
    [SerializeField] TileBuilder tileBuilder;
    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 150f);
            var roomHits = hits.ToList().Where(x => x.collider.GetComponent<Tile>() != null);
            if (roomHits.Count() != 0)
            {
                //tileBuilder.SelectTile(hits.ToList().Find(x => x.collider.GetComponent<Tile>()).collider.GetComponent<Tile>());
                tileBuilder.DoCommand(null);
            }
            else
            {
                //tileBuilder.ComletePlacing();
                tileBuilder.DoCommand(null);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            //tileBuilder.ComletePlacing();
            tileBuilder.DoCommand(null);
        }
        // FIXME хз как это правильно реализовать
        if (tileBuilder.IsTileSelected())
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                //tileBuilder.MoveSelectedTile(Direction.Up);
                tileBuilder.DoCommand(null);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                //tileBuilder.MoveSelectedTile(Direction.Left);
                tileBuilder.DoCommand(null);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                //tileBuilder.MoveSelectedTile(Direction.Down);
                tileBuilder.DoCommand(null);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                //tileBuilder.MoveSelectedTile(Direction.Right);
                tileBuilder.DoCommand(null);
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                //tileBuilder.RotateSelectedTile();
                tileBuilder.DoCommand(null);
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //tileBuilder.ChangeXMatrixPlacing(1);
            tileBuilder.DoCommand(null);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            //tileBuilder.ChangeXMatrixPlacing(-1);
            tileBuilder.DoCommand(null);
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            //tileBuilder.DeleteSelectedTile();
            tileBuilder.DoCommand(null);
        }
    }
}

