using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    [SerializeField] WallPlacementRules wallPlacementRules;

    public void PlaceRoom(Room room, Vector2Int position)
    {

    }

    public Room TakeRoom(Vector2Int position)
    {
        throw new NotImplementedException();
    }
}
