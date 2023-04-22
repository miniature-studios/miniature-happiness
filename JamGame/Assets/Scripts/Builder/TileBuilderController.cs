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
                var tile = hits.ToList().Find(x => x.collider.GetComponent<Tile>()).collider.GetComponent<Tile>();
                var command = new SelectTileCommand(tileBuilder, tile);
                _ = tileBuilder.Execute(command);
            }
            else
            {
                _ = tileBuilder.Execute(new ComplatePlacingCommand(tileBuilder));
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            _ = tileBuilder.Execute(new ComplatePlacingCommand(tileBuilder));
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            var command = new MoveSelectedTileCommand(tileBuilder, Direction.Up);
            _ = tileBuilder.Execute(command);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            var command = new MoveSelectedTileCommand(tileBuilder, Direction.Left);
            _ = tileBuilder.Execute(command);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            var command = new MoveSelectedTileCommand(tileBuilder, Direction.Down);
            _ = tileBuilder.Execute(command);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            var command = new MoveSelectedTileCommand(tileBuilder, Direction.Right);
            _ = tileBuilder.Execute(command);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            _ = tileBuilder.Execute(new RotateSelectedTileCommand(tileBuilder));
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            GameObject destroyedtile = null;
            var command = new DeleteSelectedTileCommand(tileBuilder, ref destroyedtile);
            _ = tileBuilder.Execute(command);
        }
    }
    public bool CreateTile(GameObject tilePrefab)
    {
        var command = new AddTileToSceneCommand(tileBuilder, tilePrefab);
        return tileBuilder.Execute(command);
    }
}

