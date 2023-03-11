using System.Collections.Generic;
using UnityEngine;

public class PathfindingProvider
{
    public List<Vector2Int> FindPath(Vector2Int from)
    {
        return new List<Vector2Int>();
    }
}

public partial class Location : MonoBehaviour
{
    public Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();
    public PathfindingProvider PathfindingProvider;
}

public partial class Location : MonoBehaviour
{
    [SerializeField] Employee employeePrototype;

    // TODO: Keep updated.
    List<NeedProvider> needProviders;

    void Start()
    {
        needProviders = new List<NeedProvider>(transform.GetComponentsInChildren<NeedProvider>());
    }

    public NeedProvider.Slot TryBookSlotInNeedProvider(Employee employee, NeedType need_type)
    {
        foreach (var np in needProviders)
            if (np.NeedType == need_type)
            {
                var slot = np.TryBookSlot(employee);
                if (slot != null)
                    return slot;
            }

        return null;
    }

    public void AddEmployee()
    {

    }
}
