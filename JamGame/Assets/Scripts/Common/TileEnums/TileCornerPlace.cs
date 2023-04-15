using System;
using UnityEngine;

namespace Common
{
    public enum TileCornerPlace
    {
        right_up,
        right_down,
        left_down,
        left_up
    }
    public static class TileCornerPlaceTools
    {
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
}
