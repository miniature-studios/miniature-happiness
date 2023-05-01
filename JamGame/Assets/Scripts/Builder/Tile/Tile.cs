using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WallPrefabHandler
{
    public TileWallType Type;
    public GameObject Prefab;
}
[Serializable]
public class WallCollection
{
    public Direction Place;
    public List<WallPrefabHandler> Handlers;
    public void SetWall(TileWallType type)
    {
        foreach (var handler in Handlers)
        {
            if(handler.Type == type)
                handler.Prefab.SetActive(true);
            else
                handler.Prefab.SetActive(false);
        }
    }
    public bool IsActive(TileWallType tileWallType)
    {
        if(Handlers.Find(x => x.Type == tileWallType) == null)
            return false;
        return Handlers.Find(x => x.Type == tileWallType).Prefab.gameObject.activeSelf;
    }
}

[Serializable]
public class CornerPrefabHandler
{
    public TileCornerType Type;
    public GameObject Prefab;
}
[Serializable]
public class CornerCollection
{
    public Direction Place;
    public List<CornerPrefabHandler> Handlers;
    public void SetCorner(TileCornerType type)
    {
        foreach (var handler in Handlers)
        {
            if (handler.Type == type)
                handler.Prefab.SetActive(true);
            else
                handler.Prefab.SetActive(false);
        }
    }
}

[Serializable]
public class Tile : MonoBehaviour
{
    // Changes only when changed rotation or in awake
    Dictionary<Direction, List<TileWallType>> cachedWalls;

    [SerializeField] Vector2Int position = new(0,0);
    [SerializeField] int rotation = 0;

    // Public fields for unity Editor in inspector
    [SerializeField] public TileElementsHandler elementsHandler;
    [SerializeField] List<string> marks;
    [SerializeField] public List<WallCollection> walls = new();
    [SerializeField] public List<CornerCollection> corners = new();

    public Vector2Int Position
    {
        get { return position; }
        set
        {
            position = value;
            transform.localPosition = new Vector3(
                BuilderMatrix.Step * position.y,
                transform.localPosition.y,
                -BuilderMatrix.Step * position.x
                );
        }
    }
    public int Rotation
    {
        get { return rotation; }
        set
        {
            while (value < 0)
                value += 4;
            while (value > 3)
                value -= 4;
            transform.rotation = Quaternion.Euler(0, 90 * value, 0);
            rotation = value;
            UpdateWallsCache();
        }
    }
    public IEnumerable<string> Marks { get { return marks; } }
    public Dictionary<Direction, List<TileWallType>> Walls { 
        get {
            if (cachedWalls == null)
                UpdateWallsCache();
            return cachedWalls; 
        } 
    }

    public class UpdateWallsResult
    {
        Tile tile;
        public Dictionary<Direction, TileWallType?> result = new();
        public UpdateWallsResult(Tile tile)
        {
            this.tile = tile;
        }
        public void UpdateWalls()
        {
            foreach (var pair in result)
            {
                tile.GetWallCollection(pair.Key).SetWall(pair.Value.Value);
            }
        }
        public bool Valid
        {
            get
            {
                return result.All(x => x.Value != null);
            }
        }
    }
    public UpdateWallsResult UpdateWalls(TileBuilder tileBuilder, Vector2Int myPosition)
    {
        UpdateWallsResult result = new(this);
        Direction direction = Direction.Up;
        do
        {
            TileWallType? wallTypeToPlace = TileWallType.none;
            if (tileBuilder.TilesDictionary.TryGetValue(myPosition + direction.ToVector2Int(), out var outTile))
            {
                wallTypeToPlace = tileBuilder.WallSolver.ChooseWall(
                    Marks,
                    Walls[direction],
                    outTile.Marks,
                    outTile.Walls[direction.GetOpposite()]
                );
            }
            result.result.Add(direction, wallTypeToPlace);
            direction = direction.Rotate90();
        } while (direction != Direction.Up);
        return result;
    }
    WallCollection GetWallCollection(Direction imaginePlace)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
        return walls.Find(x => x.Place == imaginePlace);
    }

    public void UpdateCorners(TileBuilder tileBuilder, Vector2Int myPosition)
    {
        Direction direction = Direction.Left;
        do
        {
            TileCornerType cornerTypeToPlace = TileCornerType.none;
            if (tileBuilder.TilesDictionary.TryGetValue(myPosition + direction.ToVector2Int(), out Tile tile1) &&
                tileBuilder.TilesDictionary.TryGetValue(myPosition + direction.Rotate45().ToVector2Int(), out Tile tile2) &&
                tileBuilder.TilesDictionary.TryGetValue(myPosition + direction.Rotate90().ToVector2Int(), out Tile tile3))
            {
                cornerTypeToPlace = ChooseCorner(
                    IsWall(direction),
                    tile1.IsWall(direction.Rotate90()),
                    tile2.IsWall(direction.GetOpposite()),
                    tile3.IsWall(direction.RotateMinus90())
                    );
            }
            GetCornerCollection(direction.Rotate45()).SetCorner(cornerTypeToPlace);
            direction = direction.Rotate90();
        } while (direction != Direction.Left);
    }
    TileCornerType ChooseCorner(bool existWall1, bool existWall2, bool existWall3, bool existWall4)
    {
        if (existWall1 == true && existWall4 == true)
            return TileCornerType.inside;
        else if (existWall1 == true && existWall3 == true)
            return TileCornerType.wall_right;
        else if (existWall4 == true && existWall2 == true)
            return TileCornerType.wall_left;
        else if (existWall2 == true && existWall3 == true)
            return TileCornerType.outside_middle;
        else if (existWall1 == true && existWall2 == true)
            return TileCornerType.outside_right;
        else if (existWall4 == true && existWall3 == true)
            return TileCornerType.outside_left;
        else 
            return TileCornerType.none;
    }
    CornerCollection GetCornerCollection(Direction imaginePlace)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
        return corners.Find(x => x.Place == imaginePlace);
    }

    bool IsWall(Direction imaginePlace)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
        var wallCollection = walls.Find(x => x.Place == imaginePlace);
        return !wallCollection.IsActive(TileWallType.none);
    }
    void UpdateWallsCache()
    {
        Dictionary<Direction, List<TileWallType>> list = new();
        foreach (var wall in walls)
        {
            Direction imagine_place = wall.Place;
            Enumerable.Range(0, rotation).ToList().ForEach(arg => imagine_place = imagine_place.Rotate90());
            list.Add(imagine_place, wall.Handlers.Select(x => x.Type).ToList());
        }
        cachedWalls = list;
    }
}