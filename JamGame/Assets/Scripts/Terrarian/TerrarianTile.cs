using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
struct TTWallMap
{
    public TerrarianTileWallObject wall_type;
    public GameObject wall_object;
}
[Serializable]
struct TTWallCollection
{
    public WallTilePlace wall_place;
    public List<TTWallMap> wall_map;
}

[Serializable]
struct TTCornerMap
{
    public TerrarianTileCornerObject corner_type;
    public GameObject corner_object;
}
[Serializable]
struct TTCornerCollection
{
    public CornerTilePlace corner_place;
    public List<TTCornerMap> corner_map;
}

[Serializable]
struct TTCenterMap
{
    public TerrarianTileCenterObject center_object_type;
    public GameObject center_object;
}

public class TerrarianTileInfo
{
    public List<TerrarianTileMark> marks;
    Dictionary<WallTilePlace, List<TerrarianTileWallObject>> walls;
    Dictionary<CornerTilePlace, List<TerrarianTileCornerObject>> corners;
    public List<TerrarianTileCenterObject> center_objects;
    public TerrarianTileInfo(List<TerrarianTileMark> marks, Dictionary<WallTilePlace, List<TerrarianTileWallObject>> walls, Dictionary<CornerTilePlace, List<TerrarianTileCornerObject>> corners, List<TerrarianTileCenterObject> center_objects)
    {
        this.marks = marks;
        this.walls = walls;
        this.corners = corners;
        this.center_objects = center_objects;
    }
    public List<TerrarianTileWallObject> GetWalls(WallTilePlace wallTilePlace)
    {
        return walls[wallTilePlace];
    }
    public List<TerrarianTileCornerObject> GetCorners(CornerTilePlace cornerTilePlace)
    {
        return corners[cornerTilePlace];
    }
}

public class TerrarianTile : MonoBehaviour
{
    /// <summary>
    /// 0 deg = 0,
    /// 90 deg = 1,
    /// 180 deg = 2,
    /// 270 deg = 3
    /// </summary>
    public int rotation = 0;
    public float step = 5;
    [SerializeField] List<TerrarianTileMark> marks;
    [SerializeField] List<TTWallCollection> walls = new();
    [SerializeField] List<TTCornerCollection> corners = new();
    [SerializeField] List<TTCenterMap> center_objects = new();
    public void Move(Direction direction)
    {
        Vector2Int _direction = direction.ToVector2Int();
        transform.position = new Vector3(
            transform.position.x + step * _direction.x,
            transform.position.y,
            transform.position.z + step * _direction.y
            );
    }
    public void Rotate(int ticks)
    {
        transform.Rotate(0.0f, ticks * 90.0f, 0.0f, Space.Self);
        rotation += ticks;
        while (rotation < 0.0f)
            rotation += 4;
        rotation %= 4;
    }
    /// <summary>
    /// Positions for tiles:
    /// <br>[0] [1] [2]</br> 
    /// <br>[3] [-] [4]</br> 
    /// <br>[5] [6] [7]</br> 
    /// </summary>
    /// <param name="tiles"></param>
    public void UpdateSides(List<TerrarianTileInfo> tiles)
    {
        tiles.Add(GetInfoWithRotation());
        UpdateWalls(tiles);
        UpdateCorners(tiles);
    }
    /// <summary>
    /// Positions for tiles:
    /// <br>[0] [1] [2]</br> 
    /// <br>[3] [-] [4]</br> 
    /// <br>[5] [6] [7]</br> 
    /// </summary>
    /// <param name="tiles"></param>
    void UpdateWalls(List<TerrarianTileInfo> tiles)
    {
        List<List<int>> walls_seq = new() {
            new () { 8, 1 },
            new () { 8, 4 },
            new () { 8, 6 },
            new () { 8, 3 }
        };
        WallTilePlace my_wallTilePlace = WallTilePlace.up;
        WallTilePlace outside_wallTilePlace = WallTilePlace.down;
        foreach (var indexes in walls_seq)
        {
            if (tiles[indexes[0]] != null && tiles[indexes[1]] != null) {
                SetWall(my_wallTilePlace,
                    ChooseWall(
                        tiles[indexes[0]].marks,
                        tiles[indexes[0]].GetWalls(my_wallTilePlace),
                        tiles[indexes[1]].marks,
                        tiles[indexes[1]].GetWalls(outside_wallTilePlace)
                        )
                    ); 
            }
            else
            {
                SetWall(my_wallTilePlace, TerrarianTileWallObject.none);
            }
            my_wallTilePlace = my_wallTilePlace.Rotate90();
            outside_wallTilePlace = outside_wallTilePlace.Rotate90();
        }
    }
    /// <summary>
    /// Function that decide what wall will be placed on first tile between two tiles 
    /// </summary>
    /// <returns></returns>
    TerrarianTileWallObject ChooseWall(List<TerrarianTileMark> TTmark_1, List<TerrarianTileWallObject> TTavailable_walls_1, List<TerrarianTileMark> TTmark_2, List<TerrarianTileWallObject> TTavailable_walls_2)
    {
        // Marks: yard, build
        // Wall Types: window, wall, door, none
        if (TTmark_1.Contains(TerrarianTileMark.build) || (TTmark_1.Contains(TerrarianTileMark.yard) && TTmark_2.Contains(TerrarianTileMark.yard)))
        {
            return TerrarianTileWallObject.none;
        }
        else if (TTmark_1.Contains(TerrarianTileMark.yard_door))
        {
            if (TTavailable_walls_1.Contains(TerrarianTileWallObject.door))
                return TerrarianTileWallObject.door;
            else
                return TerrarianTileWallObject.wall;
        }
        else if (TTmark_1.Contains(TerrarianTileMark.yard_window))
        {
            if (TTavailable_walls_1.Contains(TerrarianTileWallObject.window))
                return TerrarianTileWallObject.window;
            else
                return TerrarianTileWallObject.wall;
        }
        else
        {
            return TerrarianTileWallObject.wall;
        }
    }
    /// <summary>
    /// Positions for tiles:
    /// <br>[0] [1] [2]</br> 
    /// <br>[3] [-] [4]</br> 
    /// <br>[5] [6] [7]</br> 
    /// </summary>
    /// <param name="tiles"></param>
    void UpdateCorners(List<TerrarianTileInfo> tiles)
    {
        List<List<int>> corner_seq = new()
        {
            new () { 3, 0, 1, 8 },
            new () { 1, 2, 4, 8 },
            new () { 4, 7, 6, 8 },
            new () { 6, 5, 3, 8 },
        };
        CornerTilePlace cornerTilePlace = CornerTilePlace.left_up;
        foreach (var indexes in corner_seq)
        {
            if (tiles[indexes[0]] != null && tiles[indexes[1]] != null && tiles[indexes[2]] != null && tiles[indexes[3]] != null)
            {
                SetCorner(
                    cornerTilePlace,
                    ChooseCorner(
                        tiles[indexes[0]].marks,
                        tiles[indexes[1]].marks,
                        tiles[indexes[2]].marks,
                        tiles[indexes[3]].marks
                        )
                    );
            }
            else
            {
                SetCorner(cornerTilePlace, TerrarianTileCornerObject.none);
            }
            cornerTilePlace = cornerTilePlace.Rotate90();
        }
    }
    /// <summary>
    /// Function that decide what corner will be placed on left up position in fourth tile.
    /// <br>2 | 3</br> 
    /// <br>- + -</br> 
    /// <br>1 | 4</br> 
    /// </summary>
    /// <returns></returns>
    TerrarianTileCornerObject ChooseCorner(List<TerrarianTileMark> TTmark_1, List<TerrarianTileMark> TTmark_2, List<TerrarianTileMark> TTmark_3, List<TerrarianTileMark> TTmark_4)
    {
        // Marks: yard, build
        // Corner Types: inside, wall_left, wall_right, outside_left, otside_middle, outside_right
        if (TTmark_1.Contains(TerrarianTileMark.build) && TTmark_2.Contains(TerrarianTileMark.build) &&
            TTmark_3.Contains(TerrarianTileMark.build) && TTmark_4.Contains(TerrarianTileMark.yard))
        {
            return TerrarianTileCornerObject.inside;
        }
        if (TTmark_1.Contains(TerrarianTileMark.build) && TTmark_2.Contains(TerrarianTileMark.yard) &&
            TTmark_3.Contains(TerrarianTileMark.build) && TTmark_4.Contains(TerrarianTileMark.yard))
        {
            return TerrarianTileCornerObject.inside;
        }
        if (TTmark_1.Contains(TerrarianTileMark.build) && TTmark_2.Contains(TerrarianTileMark.build) &&
            TTmark_3.Contains(TerrarianTileMark.yard) && TTmark_4.Contains(TerrarianTileMark.yard))
        {
            return TerrarianTileCornerObject.wall_left;
        }
        if (TTmark_1.Contains(TerrarianTileMark.yard) && TTmark_2.Contains(TerrarianTileMark.build) &&
            TTmark_3.Contains(TerrarianTileMark.build) && TTmark_4.Contains(TerrarianTileMark.yard))
        {
            return TerrarianTileCornerObject.wall_right;
        }
        if (TTmark_1.Contains(TerrarianTileMark.build) && TTmark_2.Contains(TerrarianTileMark.yard) &&
            TTmark_3.Contains(TerrarianTileMark.yard) && TTmark_4.Contains(TerrarianTileMark.yard))
        {
            return TerrarianTileCornerObject.outside_left;
        }
        if (TTmark_1.Contains(TerrarianTileMark.yard) && TTmark_2.Contains(TerrarianTileMark.build) &&
            TTmark_3.Contains(TerrarianTileMark.yard) && TTmark_4.Contains(TerrarianTileMark.yard))
        {
            return TerrarianTileCornerObject.outside_middle;
        }
        if (TTmark_1.Contains(TerrarianTileMark.yard) && TTmark_2.Contains(TerrarianTileMark.yard) &&
            TTmark_3.Contains(TerrarianTileMark.build) && TTmark_4.Contains(TerrarianTileMark.yard))
        {
            return TerrarianTileCornerObject.outside_right;
        }
        return TerrarianTileCornerObject.none;
    }
    /// <summary>
    /// Turns on only wall that given, and turns off all other walls in given tile place
    /// Consider that place not rotated in base position
    /// </summary>
    /// <param name="wall_place"></param>
    /// <param name="wall_object"></param>
    void SetWall(WallTilePlace wall_place, TerrarianTileWallObject wall_object)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(arg => wall_place = wall_place.RotateMinus90());
        foreach (var wall_map in walls.Find(x => x.wall_place == wall_place).wall_map)
        {
            if(wall_map.wall_type == wall_object)
            {
                wall_map.wall_object.SetActive(true);
            }
            else
            {
                wall_map.wall_object.SetActive(false);
            }
        }
    }
    /// <summary>
    /// Turns on only corner that given, and turns off all other corners in given tile place
    /// /// Consider that place not rotated in base position
    /// </summary>
    /// <param name="corner_place"></param>
    /// <param name="corner_object"></param>
    void SetCorner(CornerTilePlace corner_place, TerrarianTileCornerObject corner_object)
    {
        Enumerable.Range(0, rotation).ToList().ForEach(arg => corner_place = corner_place.RotateMinus90());
        foreach (var corner_map in corners.Find(x => x.corner_place == corner_place).corner_map)
        {
            if (corner_map.corner_type == corner_object)
            {
                corner_map.corner_object.SetActive(true);
            }
            else
            {
                corner_map.corner_object.SetActive(false);
            }
        }
    }
    public TerrarianTileInfo GetInfoWithRotation()
    {
        Dictionary<WallTilePlace, List<TerrarianTileWallObject>> walls = new();
        foreach (var wall in this.walls)
        {
            WallTilePlace correct_place = wall.wall_place;
            Enumerable.Range(0, rotation).ToList().ForEach(arg => correct_place = correct_place.Rotate90());
            walls.Add(correct_place, wall.wall_map.Select(x => x.wall_type).ToList());
        }

        Dictionary<CornerTilePlace, List<TerrarianTileCornerObject>> corners = new();
        foreach (var corner in this.corners)
        {
            CornerTilePlace correct_place = corner.corner_place;
            Enumerable.Range(0, rotation).ToList().ForEach(arg => correct_place = correct_place.Rotate90());
            corners.Add(correct_place, corner.corner_map.Select(x => x.corner_type).ToList());
        }

        List<TerrarianTileCenterObject> center_objects = this.center_objects.Select(x => x.center_object_type).ToList();

        return new TerrarianTileInfo(marks, walls, corners, center_objects);
    }
}

public enum WallTilePlace
{
    up,
    right,
    down,
    left
}
public static class WallTilePlaceTools
{
    /// <summary>
    /// Returns next diraction, as if will be rotate by 90 degrees clockwise
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static WallTilePlace Rotate90(this WallTilePlace place)
    {
        switch (place)
        {
            case WallTilePlace.up: return WallTilePlace.right;
            case WallTilePlace.right: return WallTilePlace.down;
            case WallTilePlace.down: return WallTilePlace.left;
            case WallTilePlace.left: return WallTilePlace.up;
            default:
                Debug.LogError("Invald place");
                throw new ArgumentException();
        }
    }
    /// <summary>
    /// Returns next diraction, as if will be rotate by 90 degrees counter-clockwise
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static WallTilePlace RotateMinus90(this WallTilePlace place)
    {
        switch (place)
        {
            case WallTilePlace.up: return WallTilePlace.left;
            case WallTilePlace.left: return WallTilePlace.down;
            case WallTilePlace.down: return WallTilePlace.right;
            case WallTilePlace.right: return WallTilePlace.up;
            default:
                Debug.LogError("Invald place");
                throw new ArgumentException();
        }
    }
}
public enum CornerTilePlace
{
    right_up,
    right_down,
    left_down,
    left_up
}
public static class CornerTilePlaceTools
{
    /// <summary>
    /// Returns next diraction, as if will be rotate by 90 degrees clockwise
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static CornerTilePlace Rotate90(this CornerTilePlace place)
    {
        switch (place)
        {
            case CornerTilePlace.right_up: return CornerTilePlace.right_down;
            case CornerTilePlace.right_down: return CornerTilePlace.left_down;
            case CornerTilePlace.left_down: return CornerTilePlace.left_up;
            case CornerTilePlace.left_up: return CornerTilePlace.right_up;
            default:
                Debug.LogError("Invald place");
                throw new ArgumentException();
        }
    }
    /// <summary>
    /// Returns next diraction, as if will be rotate by 90 degrees counter-clockwise
    /// </summary>
    /// <param name="vec"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static CornerTilePlace RotateMinus90(this CornerTilePlace place)
    {
        switch (place)
        {
            case CornerTilePlace.right_up: return CornerTilePlace.left_up;
            case CornerTilePlace.left_up: return CornerTilePlace.left_down;
            case CornerTilePlace.left_down: return CornerTilePlace.right_down;
            case CornerTilePlace.right_down: return CornerTilePlace.right_up;
            default:
                Debug.LogError("Invald place");
                throw new ArgumentException();
        }
    }
}
public enum TerrarianTileMark
{
    yard,
    yard_window,
    yard_door,
    build
}
public enum TerrarianTileCornerObject
{
    inside,
    wall_left,
    wall_right,
    outside_left,
    outside_middle,
    outside_right,
    none
}
public enum TerrarianTileWallObject
{
    window,
    wall,
    door,
    none
}
public enum TerrarianTileCenterObject
{
    concrete,
    none
}