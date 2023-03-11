using System;
using System.Collections.Generic;
using UnityEngine;

public enum NeedProviderFilterType
{
    BlackList,
    WhiteList,
    None
}

public class NeedProviderFilter
{
    public NeedProviderFilter(List<Employee> employees, NeedProviderFilterType filter_type)
    {
        Employees = employees;
        FilterType = filter_type;
    }

    public List<Employee> Employees = new List<Employee>();
    public NeedProviderFilterType FilterType;

    public bool IsEmployeeAllowed(Employee employee)
    {
        switch (FilterType)
        {
            case NeedProviderFilterType.None:
                return true;
            case NeedProviderFilterType.WhiteList:
                return Employees.Contains(employee);
            case NeedProviderFilterType.BlackList:
                return !Employees.Contains(employee);
            default:
                Debug.LogError("Unknown NeedProviderFilterType!");
                return true;
        }
    }
}

[RequireComponent(typeof(Room))]
public class NeedProvider : MonoBehaviour
{
    public class Slot
    {
        public Room Room;

        public Slot(Room room)
        {
            Room = room;
        }
    }

    // TODO: Store slots as List<Slot>
    // TODO: Manage free slots

    [SerializeField] int slotAmount;

    public NeedProviderFilter Filter
        = new NeedProviderFilter(new List<Employee>(), NeedProviderFilterType.None);
    public NeedType NeedType;

    List<Employee> queuedEmployees = new List<Employee>();
    Room room;

    private void Start()
    {
        room = GetComponent<Room>();
    }

    public Slot TryBookSlot(Employee employee)
    {
        if (queuedEmployees.Count >= slotAmount)
            return null;

        if (!Filter.IsEmployeeAllowed(employee))
            return null;

        return new Slot(room);
    }

    public Need CreateNeed()
    {
        return new Need(NeedType);
    }
}
