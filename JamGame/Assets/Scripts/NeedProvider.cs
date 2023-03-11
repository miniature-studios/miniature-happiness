using System;
using System.Collections.Generic;
using UnityEngine;

public enum NeedProviderFilterType
{
    BlackList,
    WhiteList,
    FirstToTake,
    None,
}

[Serializable]
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
            case NeedProviderFilterType.FirstToTake:
                if (Employees.Contains(employee))
                    return true;
                else if (Employees.Count == 0)
                {
                    Employees.Add(employee);
                    return true;
                }
                return false;
            default:
                Debug.LogError("Unknown NeedProviderFilterType!");
                return true;
        }
    }
}

[RequireComponent(typeof(Room))]
public class NeedProvider : MonoBehaviour
{
    [Serializable]
    public class Slot
    {
        public Room Room;
        NeedProvider needProvider;

        internal Slot(Room room, NeedProvider need_provider)
        {
            Room = room;
            needProvider = need_provider;
        }

        public void Free()
        {
            needProvider.ReturnSlot(this);
        }
    }

    [SerializeField] int slotAmount;

    [SerializeField] List<Slot> availableSlots;

    public NeedProviderFilter Filter
        = new NeedProviderFilter(new List<Employee>(), NeedProviderFilterType.None);
    public NeedType NeedType;

    Room room;

    private void Start()
    {
        room = GetComponent<Room>();

        availableSlots = new List<Slot>();
        for (int i = 0; i < slotAmount; i++)
            availableSlots.Add(new Slot(room, this));
    }

    public Slot TryBookSlot(Employee employee)
    {
        if (availableSlots.Count == 0)
            return null;

        if (!Filter.IsEmployeeAllowed(employee))
            return null;

        var slot = availableSlots[0];
        availableSlots.RemoveAt(0);
        return slot;
    }

    public Need CreateNeed()
    {
        return new Need(NeedType);
    }

    void ReturnSlot(Slot slot)
    {
        availableSlots.Add(slot);
    }
}
