using Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationCell : MonoBehaviour
{
    public bool IsBusy = false;
    public bool IsPermanentBusy = false;
    public void Bind(Room room)
    {
        IsBusy = true;
    }
    public void BuildPermanentRoom(Vector3 position)
    {
        // Load GameObject
        IsPermanentBusy = true;
    }
}
