using System;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingProvider 
{
    // FindPath -> Point[]

}

public partial class Location : MonoBehaviour
{
    public Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();
    public PathfindingProvider PathfindingProvider;
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
