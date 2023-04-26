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
    public TileWallPlace Place;
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
    public TileCornerPlace Place;
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
    Dictionary<TileWallPlace, List<TileWallType>> cached_walls;

    [SerializeField] Vector2Int position = new(0,0);
    [SerializeField] int rotation = 0;
    float step = 5;
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
                step * position.y,
                transform.localPosition.y,
                -step * position.x
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
            UpdateCache();
        }
    }
    public List<string> Marks { get { return marks; } }
    public Dictionary<TileWallPlace, List<TileWallType>> Walls { get { return cached_walls; } }

    public void Awake()
    {
        UpdateCache();
    }

    public void Move(Direction direction)
    {
        Position += direction.ToVector2Int();
    }

    // [0] [1] [2]
    // [3] [4] [5]
    // [6] [7] [8]
    // 4 - current tile
    public void UpdateWalls(List<Tile> tiles)
    {
        List<List<int>> CheckQueue = new() {
            new () { 4, 1 },
            new () { 4, 5 },
            new () { 4, 7 },
            new () { 4, 3 }
        };
        TileWallPlace MyPlace = TileWallPlace.up;
        TileWallPlace OutPlace = TileWallPlace.down;
        foreach (var indexes in CheckQueue)
        {
            TileWallType wallTypeToPlace = TileWallType.none;
            if (tiles[indexes[0]] != null && tiles[indexes[1]] != null)
            {
                wallTypeToPlace = WallSolver.ChooseWall(
                    tiles[indexes[0]].Marks,
                    tiles[indexes[0]].Walls[MyPlace],
                    tiles[indexes[1]].Marks,
                    tiles[indexes[1]].Walls[OutPlace]
                );
            }
            GetWallCollection(MyPlace).SetWall(wallTypeToPlace);
            MyPlace = MyPlace.Rotate90();
            OutPlace = OutPlace.Rotate90();
        }
    }
    public bool TryUpdateWalls(List<Tile> tiles)
    {
        List<List<int>> CheckQueue = new() {
            new () { 4, 1 },
            new () { 4, 5 },
            new () { 4, 7 },
            new () { 4, 3 }
        };
        TileWallPlace MyPlace = TileWallPlace.up;
        TileWallPlace OutPlace = TileWallPlace.down;
        foreach (var indexes in CheckQueue)
        {
            TileWallType wallTypeToPlace = TileWallType.none;
            if (tiles[indexes[0]] != null && tiles[indexes[1]] != null)
            {
                try
                {
                    wallTypeToPlace = WallSolver.ChooseWall(
                        tiles[indexes[0]].Marks,
                        tiles[indexes[0]].Walls[MyPlace],
                        tiles[indexes[1]].Marks,
                        tiles[indexes[1]].Walls[OutPlace]
                    );
                }
                catch
                {
                    return false;
                }
            }
            MyPlace = MyPlace.Rotate90();
            OutPlace = OutPlace.Rotate90();
        }
        return true;
    }
    public WallCollection GetWallCollection(TileWallPlace imagine_place)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imagine_place = imagine_place.RotateMinus90());
        return walls.Find(x => x.Place == imagine_place);
    }
    public void UpdateCorners(List<Tile> tiles)
    {
        List<List<int>> CheckQueue = new()
        {
            new () { 4,3,0,1 },
            new () { 4,1,2,5 },
            new () { 4,5,8,7 },
            new () { 4,7,6,3 },
        };
        TileCornerPlace CornerPlace = TileCornerPlace.left_up;
        TileWallPlace WallPlace = TileWallPlace.left;
        foreach (var indexes in CheckQueue)
        {
            TileCornerType cornerTypeToPlace = TileCornerType.none;
            if (tiles[indexes[0]] != null && tiles[indexes[1]] != null && tiles[indexes[2]] != null && tiles[indexes[3]] != null)
            {
                var bufferWallPlace = WallPlace;
                var wall1 = tiles[indexes[0]].IsWall(bufferWallPlace);

                bufferWallPlace = bufferWallPlace.Rotate90();
                var wall2 = tiles[indexes[1]].IsWall(bufferWallPlace);

                bufferWallPlace = bufferWallPlace.Rotate90();
                var wall3 = tiles[indexes[2]].IsWall(bufferWallPlace);

                bufferWallPlace = bufferWallPlace.Rotate90();
                var wall4 = tiles[indexes[3]].IsWall(bufferWallPlace);

                cornerTypeToPlace = ChooseCorner(wall1, wall2, wall3, wall4);
            }
            GetCornerCollection(CornerPlace).SetCorner(cornerTypeToPlace);
            CornerPlace = CornerPlace.Rotate90();
            WallPlace = WallPlace.Rotate90();
        }
    }
    
    //      wall3
    // wall2     wall4
    //      wall1 (me) 
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
    public CornerCollection GetCornerCollection(TileCornerPlace imaginePlace)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
        return corners.Find(x => x.Place == imaginePlace);
    }
    bool IsWall(TileWallPlace imaginePlace)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imaginePlace = imaginePlace.RotateMinus90());
        var wallCollection = walls.Find(x => x.Place == imaginePlace);
        return !wallCollection.IsActive(TileWallType.none);
    }
    private void UpdateCache()
    {
        Dictionary<TileWallPlace, List<TileWallType>> list = new();
        foreach (var wall in walls)
        {
            TileWallPlace imagine_place = wall.Place;
            Enumerable.Range(0, rotation).ToList().ForEach(arg => imagine_place = imagine_place.Rotate90());
            list.Add(imagine_place, wall.Handlers.Select(x => x.Type).ToList());
        }
        cached_walls = list;
    }
}