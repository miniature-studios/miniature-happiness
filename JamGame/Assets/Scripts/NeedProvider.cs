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

    [SerializeField] int slotAmount;

    public NeedProviderFilter Filter;
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
        throw new NotImplementedException();
    }
}
