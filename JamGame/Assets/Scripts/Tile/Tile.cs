using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WallHandler
{
    public TileWallType Type;
    public GameObject Prefab;
}
[Serializable]
public class WallPrefabCollection
{
    public TileWallPlace Place;
    public List<WallHandler> Handlers;
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
}

[Serializable]
public class CenterPrefabHandler
{
    public TileCenterType Type;
    public GameObject Prefab;
}

[Serializable]
public class Tile : MonoBehaviour
{
    // Changes only when changed rotation or in start
    Dictionary<TileWallPlace, List<TileWallType>> cashed_walls;

    Vector2Int position = new(0,0);
    int rotation = 0;
    float step = 5;
    [SerializeField] public TileType tileType;
    [SerializeField] public TileElementsHandler elementsHandler;
    [SerializeField] private List<TileMark> marks;
    [SerializeField] public List<WallPrefabCollection> walls = new();
    [SerializeField] public List<CornerCollection> corners = new();
    [SerializeField] public List<CenterPrefabHandler> centerObjects = new();
    [SerializeField] private List<TileWallType> wallsPriorityQueue = new() {
        TileWallType.none,
        TileWallType.door,
        TileWallType.window,
        TileWallType.wall
    };

    public void Awake()
    {
        UpdateCash();
    }

    public void RotateRight()
    {
        transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
        rotation++;
        rotation %= 4;
        UpdateCash();
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
            if (tiles[indexes[0]] != null && tiles[indexes[1]] != null) {
                SetWall(MyPlace,
                    ChooseWall(
                        tiles[indexes[0]].Marks,
                        tiles[indexes[0]].Walls[MyPlace],
                        tiles[indexes[1]].Marks,
                        tiles[indexes[1]].Walls[OutPlace]
                        )
                    ); 
            }
            else
            {
                SetWall(MyPlace, TileWallType.none);
            }
            MyPlace = MyPlace.Rotate90();
            OutPlace = OutPlace.Rotate90();
        }
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
            if (tiles[indexes[0]] != null && tiles[indexes[1]] != null && tiles[indexes[2]] != null && tiles[indexes[3]] != null)
            {
                SetCorner(
                    CornerPlace,
                    ChooseCorner(
                        !tiles[indexes[0]].IsActiveNone(WallPlace),
                        !tiles[indexes[1]].IsActiveNone(WallPlace.Rotate90()),
                        !tiles[indexes[2]].IsActiveNone(WallPlace.Rotate90().Rotate90()),
                        !tiles[indexes[3]].IsActiveNone(WallPlace.Rotate90().Rotate90().Rotate90())
                        )
                    );
            }
            else
            {
                SetCorner(CornerPlace, TileCornerType.none);
            }
            CornerPlace = CornerPlace.Rotate90();
            WallPlace = WallPlace.Rotate90();
        }
    }
    TileWallType ChooseWall(List<TileMark> MyMarks, List<TileWallType> MyWalls, List<TileMark> OutMarks, List<TileWallType> OutWalls)
    {
        var wall_type_intersect = MyWalls.Intersect(OutWalls).ToList();
        if(wall_type_intersect.Count == 1)
        {
            return wall_type_intersect.First();
        }
        else if (wall_type_intersect.Count > 1)
        {
            var marks_intersect = MyMarks.Intersect(OutMarks).ToList();
            foreach (var iterator in marks_intersect.Count == 0 ? Enumerable.Reverse(wallsPriorityQueue) : wallsPriorityQueue)
            {
                if (wall_type_intersect.Contains(iterator))
                    return iterator;
            }
        }
        Debug.LogError("No intersections in two rooms");
        return TileWallType.none;
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
    void SetWall(TileWallPlace place, TileWallType type)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => place = place.RotateMinus90());
        foreach (var wall_map in walls.Find(x => x.Place == place).Handlers)
        {
            if(wall_map.Type == type)
            {
                wall_map.Prefab.SetActive(true);
            }
            else
            {
                wall_map.Prefab.SetActive(false);
            }
        }
    }
    void SetCorner(TileCornerPlace place, TileCornerType type)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(arg => place = place.RotateMinus90());
        foreach (var handler in corners.Find(x => x.Place == place).Handlers)
        {
            if (handler.Type == type)
            {
                handler.Prefab.SetActive(true);
            }
            else
            {
                handler.Prefab.SetActive(false);
            }
        }
    }
    bool IsActiveNone(TileWallPlace wallPlace)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => wallPlace = wallPlace.RotateMinus90());
        return walls.Find(x => x.Place == wallPlace).Handlers.Find(x => x.Type == TileWallType.none).Prefab.activeSelf;
    }

    // Getters setters
    public Vector2Int Position
    {
        get
        {
            return position;
        }
        set
        {
            position = value;
            transform.position = new Vector3(
                step * position.y,
                transform.position.y,
                -step * position.x
                );
        }
    }
    public int Rotation
    {
        get
        {
            return rotation;
        }
        set
        {
            if (value < 0 || value > 3)
                throw new ArgumentException($"Value: {value}");
            while (rotation != value)
                RotateRight();
        }
    }
    public List<TileMark> Marks
    {
        get
        {
            return marks;
        }
    }
    public Dictionary<TileWallPlace, List<TileWallType>> Walls
    {
        get
        {
            return cashed_walls;
        }
    }

    private void UpdateCash()
    {
        Dictionary<TileWallPlace, List<TileWallType>> list = new();
        foreach (var wall in walls)
        {
            TileWallPlace correct_place = wall.Place;
            Enumerable.Range(0, rotation).ToList().ForEach(arg => correct_place = correct_place.Rotate90());
            list.Add(correct_place, wall.Handlers.Select(x => x.Type).ToList());
        }
        cashed_walls = list;
    }
}