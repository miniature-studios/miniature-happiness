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

public class NeedProvider : MonoBehaviour
{
    [SerializeField] int slotAmount;

    public NeedProviderFilter Filter;
    public NeedType NeedType;

    List<Employee> queuedEmployees = new List<Employee>();

    public bool TryBook(Employee employee)
    {
        if (queuedEmployees.Count >= slotAmount)
            return false;

        if (!Filter.IsEmployeeAllowed(employee))
            return false;

        return true;
    }

    public Need CreateNeed()
    {
        throw new NotImplementedException();
    }
}
