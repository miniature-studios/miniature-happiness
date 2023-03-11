using System.Collections.Generic;
using UnityEngine;

public class PathfindingProvider
{
    // TODO: Pathfinding
    public List<Vector2Int> FindPath(Vector2Int from, Vector2Int to)
    {
        List<Vector2Int> path = new List<Vector2Int>();

        if (from.x != to.x)
        {
            bool inc_x = from.x < to.x;
            for (int x = from.x; inc_x ? x < to.x : x > to.x; x += inc_x ? 1 : -1)
                path.Add(new Vector2Int(x, from.y));
        }

        if (from.y != to.y)
        {
            bool inc_y = from.y < to.y;
            for (int y = from.y; inc_y ? y < to.y : y > to.y; y += inc_y ? 1 : -1)
                path.Add(new Vector2Int(to.x, y));
        }

        path.Add(to);

        return path;
    }
}

public partial class Location : MonoBehaviour
{
    public Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();
    public PathfindingProvider PathfindingProvider = new PathfindingProvider();
}

public partial class Location : MonoBehaviour
{
    [SerializeField] Employee employeePrototype;

    // TODO: Keep updated.
    List<NeedProvider> needProviders;

    public Room DebugRoom0;
    public Room DebugRoom1;
    public Room DebugRoom2;
    public Room DebugRoom3;

    void Start()
    {
        needProviders = new List<NeedProvider>(transform.GetComponentsInChildren<NeedProvider>());

        rooms.Add(new Vector2Int(0, 0), DebugRoom0);
        rooms.Add(new Vector2Int(0, -1), DebugRoom1);
        rooms.Add(new Vector2Int(1, -1), DebugRoom2);
        rooms.Add(new Vector2Int(1, 0), DebugRoom3);
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
        var new_employee = Instantiate(employeePrototype, employeePrototype.transform.parent);

        foreach (var np in needProviders)
            new_employee.AddNeed(np.CreateNeed());

        new_employee.gameObject.SetActive(true);
    }
}
