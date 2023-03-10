using System;
using System.Collections.Generic;
using UnityEngine;

public class Path
{

}

public partial class Location : MonoBehaviour
{
    [SerializeField] WallPlacementRules wallPlacementRules;

    Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();

    public void PlaceRoom(Room room, Vector2Int position)
    {
        rooms.Add(position, room);
        UpdateRoomWalls(position);
    }

    public Room TakeRoom(Vector2Int position)
    {
        var room = rooms[position];
        rooms.Remove(position);
        UpdateRoomWalls(position);
        return room;
    }

    void UpdateRoomWalls(Vector2Int position)
    {
        // Send UpdateWalls requests based on wallPlacementRules to adjacent rooms
    }

    public Path FindPath()
    {
        throw new NotImplementedException();
    }
}

public partial class Location : MonoBehaviour
{
    // TODO: Keep updated.
    List<NeedProvider> needProviders;

    void Start()
    {
        needProviders = new List<NeedProvider>(transform.GetComponentsInChildren<NeedProvider>());
    }

    public bool TryBookSlotInNeedProvider(Employee employee, NeedType need_type)
    {
        foreach (var np in needProviders)
            if (np.NeedType == need_type && np.TryBook(employee))
                return true;

        return false;
    }
}
