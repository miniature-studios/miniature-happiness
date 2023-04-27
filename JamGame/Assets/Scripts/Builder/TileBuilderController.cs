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
            var hits = Physics.RaycastAll(ray, 300f);
            var roomHits = hits.ToList().Where(x => x.collider.GetComponent<Tile>() != null);
            if (roomHits.Count() != 0)
            {
                var tile = hits.ToList().Find(x => x.collider.GetComponent<Tile>()).collider.GetComponent<Tile>();
                var command = new SelectTileCommand(tileBuilder, tile);
                var answer = tileBuilder.Execute(command);
                Debug.Log(answer.Massage);
            }
            else
            {
                var answer = tileBuilder.Execute(new ComplatePlacingCommand(tileBuilder));
                Debug.Log(answer.Massage);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            var command = new CancelAddingCommand(tileBuilder);
            var answer = tileBuilder.Execute(command);
            Debug.Log(answer.Massage);
        }
        if (Input.GetMouseButtonDown(1))
        {
            var answer = tileBuilder.Execute(new ComplatePlacingCommand(tileBuilder));
            Debug.Log(answer.Massage);
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            var command = new MoveSelectedTileCommand(tileBuilder, Direction.Up);
            var answer = tileBuilder.Execute(command);
            Debug.Log(answer.Massage);
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            var command = new MoveSelectedTileCommand(tileBuilder, Direction.Left);
            var answer = tileBuilder.Execute(command);
            Debug.Log(answer.Massage);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            var command = new MoveSelectedTileCommand(tileBuilder, Direction.Down);
            var answer = tileBuilder.Execute(command);
            Debug.Log(answer.Massage);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            var command = new MoveSelectedTileCommand(tileBuilder, Direction.Right);
            var answer = tileBuilder.Execute(command);
            Debug.Log(answer.Massage);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            var answer = tileBuilder.Execute(new RotateSelectedTileCommand(tileBuilder));
            Debug.Log(answer.Massage);
        }
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            GameObject destroyedtile = null;
            var command = new DeleteSelectedTileCommand(tileBuilder, ref destroyedtile);
            var answer = tileBuilder.Execute(command);
            Debug.Log(answer.Massage);
        }
    }
    public Answer CreateTile(GameObject tilePrefab)
    {
        var command = new AddTileToSceneCommand(tileBuilder, tilePrefab);
        return tileBuilder.Execute(command);
    }
}

