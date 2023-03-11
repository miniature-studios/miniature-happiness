using Common;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingProvider
{
    Dictionary<Vector2Int, List<Direction>> availablePaths;

    public PathfindingProvider(Dictionary<Vector2Int, List<Direction>> available_paths)
    {
        availablePaths = available_paths;
    }

    // TODO: Weighted pathfinding.
    public List<Vector2Int> FindPath(Vector2Int from, Vector2Int to)
    {
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(from);

        Dictionary<Vector2Int, Vector2Int> came_from = new Dictionary<Vector2Int, Vector2Int>()
            { { from, from } };

        while (frontier.Count != 0)
        {
            var current = frontier.Dequeue();
            foreach (var direction in availablePaths[current])
            {
                var next = direction.ToVector2Int() + current;

                if (!came_from.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    came_from.Add(next, current);
                }

                if (next == to)
                {
                    var path = new List<Vector2Int>() { to };
                    while (true)
                    {
                        path.Add(came_from[path[^1]]);
                        if (path[^1] == from)
                            break;
                    }
                    path.Reverse();
                    return path;
                }
            }
        }

        Debug.LogError("Path from " + from + " to " + to + " not found!");
        return null;
    }
}

public partial class Location : MonoBehaviour
{
    public Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();
    public PathfindingProvider PathfindingProvider;
}

[RequireComponent(typeof(NeedCollectionModifier))]
public partial class Location : MonoBehaviour
{
    [SerializeField] Employee employeePrototype;

    // TODO: Keep updated.
    List<NeedProvider> needProviders;
    List<Employee> employees = new List<Employee>();

    [SerializeField] List<NeedType> activeNeeds;
    [SerializeField] NeedCollectionModifier needCollectionModifier;

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

        var available_paths = new Dictionary<Vector2Int, List<Direction>>();
        available_paths.Add(new Vector2Int(0, 0), new List<Direction>() { Direction.Right, Direction.Down });
        available_paths.Add(new Vector2Int(1, 0), new List<Direction>() { Direction.Left, Direction.Down });
        available_paths.Add(new Vector2Int(0, -1), new List<Direction>() { Direction.Up });
        available_paths.Add(new Vector2Int(1, -1), new List<Direction>() { Direction.Up });

        PathfindingProvider = new PathfindingProvider(available_paths);

        var need_parameters = new List<NeedParameters>();
        foreach (var nt in activeNeeds)
            need_parameters.Add(new NeedParameters(nt));
        need_parameters = needCollectionModifier.Apply(need_parameters);
        foreach (var np in needProviders)
            np.ProvideNeedParameters(need_parameters);
    }

    [SerializeField] List<NeedDesatisfactionSpeedModifier> needDesatisfactionSpeedModifiers;
    void Update()
    {
        foreach (var need_ty in activeNeeds)
        {
            float mul = 1.0f;
            foreach (var mod in needDesatisfactionSpeedModifiers)
                if (mod.ty == need_ty)
                {
                    mul = mod.multiplier;
                    break;
                }

            foreach (var emp in employees)
                emp.DesatisfyNeed(need_ty, Time.deltaTime * mul);
        }
    }

    // TODO: Try book closest provider
    public NeedSlot TryBookSlotInNeedProvider(Employee employee, NeedType need_type)
    {
        foreach (var np in needProviders)
            if (np.NeedType == need_type)
            {
                var booked = np.TryBook(employee);
                if (booked != null)
                    return booked;
            }

        return null;
    }

    [SerializeField] EmployeeNeeds employeeNeeds;
    public void AddEmployee()
    {
        var new_employee = Instantiate(employeePrototype, employeePrototype.transform.parent);

        foreach (var need in activeNeeds)
            new_employee.AddNeed(new NeedParameters(need));

        new_employee.gameObject.SetActive(true);
        employees.Add(new_employee);
    }
}