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
    public bool IsActiveNone()
    {
        return Handlers.Find(x => x.Type == TileWallType.none).Prefab.activeSelf;
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
    public void SetWall(TileCornerType type)
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
public class CenterPrefabHandler
{
    public TileCenterType Type;
    public GameObject Prefab;
}

[Serializable]
public class Tile : MonoBehaviour
{
    // Changes only when changed rotation or in awake
    Dictionary<TileWallPlace, List<TileWallType>> cached_walls;

    Vector2Int position = new(0,0);
    int rotation = 0;
    float step = 5;
    [SerializeField] TileElementsHandler elementsHandler;
    [SerializeField] List<string> marks;
    [SerializeField] List<WallCollection> walls = new();
    [SerializeField] List<CornerCollection> corners = new();
    [SerializeField] List<CenterPrefabHandler> centerObjects = new();
    [SerializeField] List<TileWallType> ForSameWalls_PriorityQueue = new() {
        TileWallType.none,
        TileWallType.door,
        TileWallType.window,
        TileWallType.wall
    };
    [SerializeField] List<TileWallType> ForDifferentTiles_PriorityQueue = new() {
        TileWallType.wall,
        TileWallType.window,
        TileWallType.door,
        TileWallType.none
    };

    public void Awake()
    {
        UpdateCache();
    }

    public void RotateRight()
    {
        transform.Rotate(0.0f, 90.0f, 0.0f, Space.Self);
        rotation++;
        rotation %= 4;
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
            if (tiles[indexes[0]] != null && tiles[indexes[1]] != null) {
                wallTypeToPlace = ChooseWall(
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
                cornerTypeToPlace = ChooseCorner(
                    !tiles[indexes[0]].IsActiveNone(WallPlace),
                    !tiles[indexes[1]].IsActiveNone(WallPlace.Rotate90()),
                    !tiles[indexes[2]].IsActiveNone(WallPlace.Rotate90().Rotate90()),
                    !tiles[indexes[3]].IsActiveNone(WallPlace.Rotate90().Rotate90().Rotate90())
                );
            }
            GetCornerCollection(CornerPlace).SetWall(cornerTypeToPlace);
            CornerPlace = CornerPlace.Rotate90();
            WallPlace = WallPlace.Rotate90();
        }
    }
    TileWallType ChooseWall(List<string> MyMarks, List<TileWallType> MyWalls, List<string> OutMarks, List<TileWallType> OutWalls)
    {
        var wall_type_intersect = MyWalls.Intersect(OutWalls).ToList();
        if(wall_type_intersect.Count == 1)
        {
            return wall_type_intersect.First();
        }
        else if (wall_type_intersect.Count > 1)
        {
            var marks_intersect = MyMarks.Intersect(OutMarks).ToList();
            foreach (var iterator in marks_intersect.Count == 0 ? ForDifferentTiles_PriorityQueue : ForSameWalls_PriorityQueue)
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
    public CornerCollection GetCornerCollection(TileCornerPlace imagine_place)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imagine_place = imagine_place.RotateMinus90());
        return corners.Find(x => x.Place == imagine_place);
    }
    bool IsActiveNone(TileWallPlace imagine_place)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(x => imagine_place = imagine_place.RotateMinus90());
        return walls.Find(x => x.Place == imagine_place).IsActiveNone();
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
    public List<string> Marks
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
            return cached_walls;
        }
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

    public void UpdateTilePrefabComponents()
    {
        foreach (var wallCollection in walls)
        {
            float degrees = wallCollection.Place.GetDegrees();
            foreach (var handler in wallCollection.Handlers)
            {
                var prefabHandler = elementsHandler.WallPrefabHandlers.Find(x => x.Type == handler.Type);
                if (prefabHandler != null)
                {
                    if (handler.Prefab != null)
                        DestroyImmediate(handler.Prefab);
                    handler.Prefab = Instantiate(prefabHandler.Prefab, transform.position, prefabHandler.Prefab.transform.rotation, transform);
                    handler.Prefab.transform.Rotate(new(0, degrees, 0));
                    handler.Prefab.SetActive(false);
                    handler.Prefab.name = $"Wall - {handler.Type} - {wallCollection.Place} -| " + handler.Prefab.name;
                }
                else
                {
                    Debug.LogWarning($"Cannot find prefab for {handler.Type}");
                }
            }
        }
        foreach (var cornerCollection in corners)
        {
            float degrees = cornerCollection.Place.GetDegrees();
            foreach (var handler in cornerCollection.Handlers)
            {
                var prefabHandler = elementsHandler.CornerPrefabHandlers.Find(x => x.Type == handler.Type);
                if (prefabHandler != null)
                {
                    if (handler.Prefab != null)
                        DestroyImmediate(handler.Prefab);
                    handler.Prefab = Instantiate(prefabHandler.Prefab, transform.position, prefabHandler.Prefab.transform.rotation, transform);
                    handler.Prefab.transform.Rotate(new(0, degrees, 0));
                    handler.Prefab.SetActive(false);
                    handler.Prefab.name = $"Corner - {handler.Type} - {cornerCollection.Place} -| " + handler.Prefab.name;
                }
                else
                {
                    Debug.LogWarning($"Cannot find prefab for {handler.Type}");
                }
            }
        }
        foreach (var object_in_center in centerObjects)
        {
            var prefabHandler = elementsHandler.CenterPrefabHandlers.Find(x => x.Type == object_in_center.Type);
            if (prefabHandler != null)
            {
                if (object_in_center.Prefab != null)
                    DestroyImmediate(object_in_center.Prefab);
                object_in_center.Prefab = Instantiate(prefabHandler.Prefab, transform.position, new(), transform);
                //object_in_center.Prefab.SetActive(false);
                object_in_center.Prefab.name = $"Corner - {object_in_center.Type} -| " + object_in_center.Prefab.name;
            }
            else
            {
                Debug.LogWarning($"Cannot find prefab for {object_in_center.Type}");
            }
        }
    }
}