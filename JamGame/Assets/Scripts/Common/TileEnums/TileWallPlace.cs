using System;
using UnityEngine;

namespace Common
{
    public enum TileWallPlace
    {
        up,
        right,
        down,
        left
    }

    public static class TileWallPlaceTools
    {
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
}