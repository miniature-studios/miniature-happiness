using System;
using UnityEngine;

public enum TileWallPlace
{
    up,
    right,
    down,
    left
}
public static class TileWallPlaceTools
{
    public static TileWallPlace Rotate90(this TileWallPlace place)
    {
        switch (place)
        {
            case TileWallPlace.up: return TileWallPlace.right;
            case TileWallPlace.right: return TileWallPlace.down;
            case TileWallPlace.down: return TileWallPlace.left;
            case TileWallPlace.left: return TileWallPlace.up;
            default:
                Debug.LogError("Invald place");
                throw new ArgumentException();
        }
    }
    public static float GetDegrees(this TileWallPlace place)
    {
        switch (place)
        {
            case TileWallPlace.up: return 0;
            case TileWallPlace.right: return 90;
            case TileWallPlace.down: return 180;
            case TileWallPlace.left: return 270;
            default:
                Debug.LogError("Invald place");
                throw new ArgumentException();
        }
    }
    public static TileWallPlace RotateMinus90(this TileWallPlace place)
    {
        switch (place)
        {
            case TileWallPlace.up: return TileWallPlace.left;
            case TileWallPlace.left: return TileWallPlace.down;
            case TileWallPlace.down: return TileWallPlace.right;
            case TileWallPlace.right: return TileWallPlace.up;
            default:
                Debug.LogError("Invald place");
                throw new ArgumentException();
        }
    }
}
public enum TileCornerPlace
{
    right_up,
    right_down,
    left_down,
    left_up
}
public static class TileCornerPlaceTools
{
    public static TileCornerPlace Rotate90(this TileCornerPlace place)
    {
        switch (place)
        {
            case TileCornerPlace.right_up: return TileCornerPlace.right_down;
            case TileCornerPlace.right_down: return TileCornerPlace.left_down;
            case TileCornerPlace.left_down: return TileCornerPlace.left_up;
            case TileCornerPlace.left_up: return TileCornerPlace.right_up;
            default:
                Debug.LogError("Invald place");
                throw new ArgumentException();
        }
    }
    public static float GetDegrees(this TileCornerPlace place)
    {
        switch (place)
        {
            case TileCornerPlace.right_up: return 0;
            case TileCornerPlace.right_down: return 90;
            case TileCornerPlace.left_down: return 180;
            case TileCornerPlace.left_up: return 270;
            default:
                Debug.LogError("Invald place");
                throw new ArgumentException();
        }
    }
    public static TileCornerPlace RotateMinus90(this TileCornerPlace place)
    {
        switch (place)
        {
            case TileCornerPlace.right_up: return TileCornerPlace.left_up;
            case TileCornerPlace.left_up: return TileCornerPlace.left_down;
            case TileCornerPlace.left_down: return TileCornerPlace.right_down;
            case TileCornerPlace.right_down: return TileCornerPlace.right_up;
            default:
                Debug.LogError("Invald place");
                throw new ArgumentException();
        }
    }
}
public enum TileCornerType
{
    inside,
    wall_left,
    wall_right,
    outside_left,
    outside_middle,
    outside_right,
    none
}
public enum TileWallType
{
    window,
    wall,
    door,
    none
}
public enum TileCenterType
{
    concrete,
    none
}
