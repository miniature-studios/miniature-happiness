using System;
using Common;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

class TerrarianTileCell
{
    TerrarianTile terrarianTile;
    Vector2Int position;
    public TerrarianTileCell(TerrarianTile terrarianTile, Vector2Int position)
    {
        this.terrarianTile = terrarianTile;
        this.position = position;
    }
    public TerrarianTile TerrarianTile { get => terrarianTile; }
    public int Rotation { 
        get => terrarianTile.rotation;
        set 
        {
            if (value < 0 || value > 3)
                throw new ArgumentException($"Value: {value}");
            while (terrarianTile.rotation != value)
                terrarianTile.Rotate(1);
        } 
    }
    public Vector2Int Position
    {
        get => position;
        set
        {
            position = value;
            terrarianTile.transform.position = new Vector3(value.x * terrarianTile.step, 0, value.y * terrarianTile.step);
        }
    }
    public void Move(Direction direction)
    {
        terrarianTile.Move(direction);
        position += direction.ToVector2Int();
    }
    public void Rotate()
    {
        terrarianTile.Rotate(1);
    }
}

public class TerrarianBuilder : MonoBehaviour
{
    [SerializeField] TerrarianTilePrefabsHandle terrarianTilePrefabsHandle;
    [SerializeField] bool EditorMode = true;
    [SerializeField] GameObject pointer_prefab;
    GameObject pointer;
    List<TerrarianTileCell> cells = new();
    [SerializeField] TerrarianTileCell SelectedTile = null;
    Vector2Int previous_place;
    int previous_rotation;
    public void Update()
    {
        if (EditorMode)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                AddTileToScene(TerrarianTileType.build);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                AddTileToScene(TerrarianTileType.outdoor);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                AddTileToScene(TerrarianTileType.window);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                AddTileToScene(TerrarianTileType.stairs);
            }

            if (Input.GetMouseButtonDown(0))
            {
                SelectTile_ByClick();
            }
            if (Input.GetMouseButtonDown(1))
            {
                ComletePlacing();
            }
            if (SelectedTile != null)
            {
                if (Input.GetKeyDown(KeyCode.W))
                {
                    SelectedTile.Move(Direction.Up);
                    pointer.transform.position = SelectedTile.TerrarianTile.transform.position;
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    SelectedTile.Move(Direction.Left);
                    pointer.transform.position = SelectedTile.TerrarianTile.transform.position;
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    SelectedTile.Move(Direction.Down);
                    pointer.transform.position = SelectedTile.TerrarianTile.transform.position;
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    SelectedTile.Move(Direction.Right);
                    pointer.transform.position = SelectedTile.TerrarianTile.transform.position;
                }
                if (Input.GetKeyDown(KeyCode.R))
                {
                    SelectedTile.Rotate();
                }
            }
        }
    }
    public void SelectTile_ByClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var hits = Physics.RaycastAll(ray, 150f);
        var roomHits = hits.ToList().Where(x => x.collider.GetComponent<TerrarianTile>() != null);
        if (roomHits.Count() != 0)
        {
            var NowSelectedTile = hits.ToList().Find(x => x.collider.GetComponent<TerrarianTile>()).collider.GetComponent<TerrarianTile>();
            if ((SelectedTile == null) || (SelectedTile.TerrarianTile != NowSelectedTile))
            {
                ComletePlacing();
                SelectedTile = cells.Find(x => x.TerrarianTile == NowSelectedTile);
                previous_place = SelectedTile.Position;
                previous_rotation = SelectedTile.Rotation;
                pointer = Instantiate(pointer_prefab, NowSelectedTile.transform.position, new Quaternion(), transform);
            }
        }
        else
        {
            ComletePlacing();
        }
    }
    public void ComletePlacing()
    {
        if (SelectedTile == null)
            return;
        if (cells.Where(x => x != SelectedTile).Select(x => x.Position).Contains(SelectedTile.Position)) 
        {
            SelectedTile.Position = previous_place;
            SelectedTile.Rotation = previous_rotation;
        }
        SelectedTile = null;
        Destroy(pointer.gameObject);
        UpdateAllSidesInTerrarian();
    }
    public void AddTileToScene(TerrarianTileType terrarianTileType)
    {
        var new_position = new Vector2Int(0, 0);
        while (cells.Select(x => x.Position).Contains(new_position))
        {
            new_position += Vector2Int.up;
        }
        var prefab = terrarianTilePrefabsHandle.terrarianTilePrefabMatches.Find(x => x.terrarianTileType == terrarianTileType).prefab;
        cells.Add(new TerrarianTileCell(Instantiate(prefab, transform).GetComponent<TerrarianTile>(), new_position));
        cells.Last().Position = new_position;
        UpdateAllSidesInTerrarian();
    }
    public void UpdateAllSidesInTerrarian()
    {
        foreach (var tileCell in cells)
        {
            List<TerrarianTileInfo> terrarianTileInfos = new();
            for (int i = 1; i >= -1; i--)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0)
                        continue;
                    var buffer = cells.FirstOrDefault(x => x.Position == new Vector2Int(tileCell.Position.x + j, tileCell.Position.y + i));
                    if(buffer != null)
                    {
                        terrarianTileInfos.Add(buffer.TerrarianTile.GetInfoWithRotation());
                    }
                    else
                    {
                        terrarianTileInfos.Add(null);
                    }
                }
            }
            tileCell.TerrarianTile.UpdateSides(terrarianTileInfos);
        }
    }
}

public enum TerrarianTileType
{
    build,
    outdoor,
    stairs,
    window
}
