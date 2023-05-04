using Common;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingProvider
{
    private readonly Dictionary<Vector2Int, List<Direction>> availablePaths;

    public PathfindingProvider(Dictionary<Vector2Int, List<Direction>> available_paths)
    {
        availablePaths = available_paths;
    }

    // TODO: Weighted pathfinding.
    public List<Vector2Int> FindPath(Vector2Int from, Vector2Int to)
    {
        Queue<Vector2Int> frontier = new();
        frontier.Enqueue(from);

        Dictionary<Vector2Int, Vector2Int> came_from = new()
            { { from, from } };

        while (frontier.Count != 0)
        {
            Vector2Int current = frontier.Dequeue();
            foreach (Direction direction in availablePaths[current])
            {
                Vector2Int next = direction.ToVector2Int() + current;

                if (!came_from.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    came_from.Add(next, current);
                }

                if (next == to)
                {
                    List<Vector2Int> path = new() { to };
                    while (true)
                    {
                        path.Add(came_from[path[^1]]);
                        if (path[^1] == from)
                        {
                            break;
                        }
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
    public Dictionary<Vector2Int, Room> rooms = new();
    public PathfindingProvider PathfindingProvider;
}

[RequireComponent(typeof(NeedCollectionModifier))]
public partial class Location : MonoBehaviour
{
    [SerializeField] private Employee employeePrototype;

    // TODO: Keep updated.
    private List<NeedProvider> needProviders;
    private readonly List<Employee> employees = new();

    [SerializeField] private List<NeedType> activeNeeds;
    [SerializeField] private NeedCollectionModifier needCollectionModifier;

    private void Start()
    {

    }

    public void InitGameMode()
    {
        needProviders = new List<NeedProvider>(transform.GetComponentsInChildren<NeedProvider>());

        List<NeedParameters> need_parameters = new();
        foreach (NeedType nt in activeNeeds)
        {
            need_parameters.Add(new NeedParameters(nt));
        }

        need_parameters = needCollectionModifier.Apply(need_parameters);
        foreach (NeedProvider np in needProviders)
        {
            np.ProvideNeedParameters(need_parameters);
        }
    }

    [SerializeField] private List<NeedDesatisfactionSpeedModifier> needDesatisfactionSpeedModifiers;

    private void Update()
    {
        foreach (NeedType need_ty in activeNeeds)
        {
            float mul = 1.0f;
            foreach (NeedDesatisfactionSpeedModifier mod in needDesatisfactionSpeedModifiers)
            {
                if (mod.ty == need_ty)
                {
                    mul = mod.multiplier;
                    break;
                }
            }

            foreach (Employee emp in employees)
            {
                emp.DesatisfyNeed(need_ty, Time.deltaTime * mul);
            }
        }
    }

    // TODO: Try book closest provider
    public NeedSlot TryBookSlotInNeedProvider(Employee employee, NeedType need_type)
    {
        foreach (NeedProvider np in needProviders)
        {
            if (np.NeedType == need_type)
            {
                NeedSlot booked = np.TryBook(employee);
                if (booked != null)
                {
                    return booked;
                }
            }
        }

        return null;
    }

    [SerializeField] private EmployeeNeeds employeeNeeds;
    public void AddEmployee()
    {
        Employee new_employee = Instantiate(employeePrototype, employeePrototype.transform.parent);

        foreach (NeedType need in activeNeeds)
        {
            new_employee.AddNeed(new NeedParameters(need));
        }

        new_employee.gameObject.SetActive(true);
        employees.Add(new_employee);
    }
}