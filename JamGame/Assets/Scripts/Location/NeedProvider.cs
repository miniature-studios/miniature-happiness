using System;
using System.Collections.Generic;
using System.Linq;
using Employee;
using Employee.Needs;
using UnityEngine;

namespace Location
{
    [AddComponentMenu("Scripts/Location/Location.NeedProvider")]
    public class NeedProvider : MonoBehaviour
    {
        public class PlaceInWaitingLine
        {
            public EmployeeImpl Employee => employee;

            private NeedProvider needProvider;
            private EmployeeImpl employee;
            private PlaceInWaitingLine next;

            protected PlaceInWaitingLine(
                PlaceInWaitingLine next,
                EmployeeImpl employee,
                NeedProvider needProvider
            )
            {
                this.next = next;
                this.employee = employee;
                this.needProvider = needProvider;
            }

            public EmployeeImpl GetNextInLine()
            {
                return next?.employee;
            }

            public void Drop()
            {
                PlaceInWaitingLine previous_place = needProvider.FindPreviousPlaceInWaitingLine(
                    this
                );
                if (previous_place != null)
                {
                    previous_place.next = next;
                }

                _ = needProvider.waitingLine.Remove(this);
            }

            // TODO: private
            public void RemoveNext()
            {
                next = null;
            }
        }

        private class PlaceInWaitingLineConstructor : PlaceInWaitingLine
        {
            public PlaceInWaitingLineConstructor(
                PlaceInWaitingLine next,
                EmployeeImpl employee,
                NeedProvider needProvider
            )
                : base(next, employee, needProvider) { }
        }

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
                        return Employees.Contains(employee)
                            || Employees.Count == 0
                            || (Employees.Count == 1 && Employees[0] == null);
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
                        else if (Employees.Count == 1 && Employees[0] == null)
                        {
                            Employees[0] = employee;
                        }
                        else if (Employees.Count > 1 || !Employees.Contains(employee))
                        {
                            Debug.LogError("Place is already assigned to other employee");
                        }

                        break;
                    default:
                        break;
                }
            }
        }

        [SerializeField]
        private Filter filter;

        [SerializeField]
        private bool bindToThisProviderOnFirstVisit;

        public NeedType NeedType;

        private EmployeeImpl currentEmployee = null;

        private List<PlaceInWaitingLine> waitingLine = new();

        public void Take(PlaceInWaitingLine place)
        {
            if (place == null || place.GetNextInLine() != null)
            {
                Debug.LogError("Invalid waiting line place provided");
                return;
            }

            // TODO: Do we need to check criterias again?
            EmployeeImpl employee = place.Employee;
            currentEmployee = employee;

            foreach (NeedModifiers modifier in registeredModifiers)
            {
                currentEmployee.RegisterModifier(modifier);
            }

            if (bindToThisProviderOnFirstVisit)
            {
                employee.BindToNeedProvider(this);
            }
        }

        // TODO: Control release inside NeedProvider
        public void Release()
        {
            foreach (NeedModifiers modifier in registeredModifiers)
            {
                currentEmployee.UnregisterModifier(modifier);
            }

            if (waitingLine[0].Employee != currentEmployee)
            {
                Debug.LogError("Wrong waiting line construction detected");
                return;
            }
            waitingLine.RemoveAt(0);

            if (waitingLine.Count > 0)
            {
                waitingLine[0].RemoveNext();
            }
        }

        public bool IsAvailable(EmployeeImpl employee)
        {
            return filter.IsEmployeeAllowed(employee);
        }

        public PlaceInWaitingLine TryLineUp(EmployeeImpl employee)
        {
            if (!filter.IsEmployeeAllowed(employee))
            {
                return null;
            }

            foreach (PlaceInWaitingLine wl_place in waitingLine)
            {
                if (wl_place.Employee == employee)
                {
                    Debug.LogError("Employee is already in waiting line");
                }
            }

            filter.Take(employee);

            PlaceInWaitingLine last = null;
            if (waitingLine.Count != 0)
            {
                last = waitingLine[^1];
            }

            PlaceInWaitingLineConstructor place = new(last, employee, this);
            waitingLine.Add(place);
            return place;
        }

        private PlaceInWaitingLine FindPreviousPlaceInWaitingLine(PlaceInWaitingLine current)
        {
            foreach (PlaceInWaitingLine place in waitingLine)
            {
                if (place.GetNextInLine() == current.Employee)
                {
                    return place;
                }
            }

            return null;
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

        private void OnDrawGizmos()
        {
            if (waitingLine.Any())
            {
                List<Vector3> points = new() { transform.position };
                foreach (PlaceInWaitingLine place in waitingLine)
                {
                    points.Add(place.Employee.transform.position);
                }

                Gizmos.color = Color.red;
                Gizmos.DrawLineStrip(new ReadOnlySpan<Vector3>(points.ToArray()), false);
            }
        }
    }
}
