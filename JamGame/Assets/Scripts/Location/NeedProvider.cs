using Employee;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Location
{
    [AddComponentMenu("Location.NeedProvider")]
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
            public Filter(List<EmployeeImpl> employees, FilterType filter_type)
            {
                Employees = employees;
                FilterType = filter_type;
            }

            public List<EmployeeImpl> Employees = new();
            public FilterType FilterType;

            public bool IsEmployeeAllowed(EmployeeImpl employee)
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

            public void Take(EmployeeImpl employee)
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

        [SerializeField]
        private Filter filter;

        public NeedType NeedType;

        private EmployeeImpl currentEmployee = null;

        private readonly List<NeedModifiers> registeredModifiers = new();

        public bool TryTake(EmployeeImpl employee)
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

        public bool IsAvailable(EmployeeImpl employee)
        {
            return filter.IsEmployeeAllowed(employee);
        }

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
}
