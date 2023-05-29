using System;
using System.Collections.Generic;
using UnityEngine;

public class NeedProvider : MonoBehaviour
{
    public enum FilterType
    {
        None,
        BlackList,
        WhiteList,
        FirstToTake,
    }

    [Serializable]
    public class Filter
    {
        public Filter(List<Employee> employees, FilterType filter_type)
        {
            Employees = employees;
            FilterType = filter_type;
        }

        public List<Employee> Employees = new();
        public FilterType FilterType;

        public bool IsEmployeeAllowed(Employee employee)
        {
            switch (FilterType)
            {
                case FilterType.None:
                    return true;
                case FilterType.WhiteList:
                    return Employees.Contains(employee);
                case FilterType.BlackList:
                    return !Employees.Contains(employee);
                case FilterType.FirstToTake:
                    return Employees.Contains(employee) || Employees.Count == 0;
                default:
                    Debug.LogError("Unknown NeedProviderFilterType!");
                    return false;
            }
        }

        public void Take(Employee employee)
        {
            switch (FilterType)
            {
                case FilterType.FirstToTake:
                    if (Employees.Count == 0)
                    {
                        Employees.Add(employee);
                    }

                    if (Employees.Count > 1 || !Employees.Contains(employee))
                    {
                        Debug.LogError("Place is already assigned to other employee");
                        break;
                    }

                    break;
                default:
                    break;
            }
        }
    }

    [SerializeField] private Filter filter;
    [SerializeField] public NeedType NeedType;

    private Employee currentEmployee = null;

    private void Start()
    {

    }

    public bool TryTake(Employee employee)
    {
        if (!IsAvailable(employee))
        {
            return false;
        }

        filter.Take(employee);
        currentEmployee = employee;

        foreach (NeedModifiers modifier in registeredModifiers)
        {
            currentEmployee.RegisterModifier(modifier);
        }

        return true;
    }

    // TODO: Control release inside NeedProvider 
    public void Release()
    {
        foreach (NeedModifiers modifier in registeredModifiers)
        {
            currentEmployee.UnregisterModifier(modifier);
        }
    }

    public bool IsAvailable(Employee employee)
    {
        return filter.IsEmployeeAllowed(employee);
    }

    private readonly List<NeedModifiers> registeredModifiers = new();

    public void RegisterModifier(NeedModifiers modifiers)
    {
        if (registeredModifiers.Contains(modifiers))
        {
            Debug.LogWarning("Modifiers already registered");
            return;
        }

        registeredModifiers.Add(modifiers);
    }

    public void UnregisterModifier(NeedModifiers modifiers)
    {
        if (!registeredModifiers.Remove(modifiers))
        {
            Debug.LogWarning("Modifiers to unregister not found");
        }
    }
}
