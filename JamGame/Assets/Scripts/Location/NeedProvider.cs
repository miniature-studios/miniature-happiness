using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Employee;
using Employee.Needs;
using Level.Boss.Task;
using Level.GlobalTime;
using UnityEngine;
using UnityEngine.Events;

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

            public void OnEmployeeFired(EmployeeImpl employee)
            {
                _ = Employees.Remove(employee);
            }
        }

        [SerializeField]
        private Filter filter;

        [SerializeField]
        private bool bindToThisProviderOnFirstVisit;
        public bool BindToThisNeedProviderOnFirstVisit => bindToThisProviderOnFirstVisit;

        public NeedType NeedType;

        private EmployeeImpl currentEmployee = null;
        private RealTimeSeconds currentEmployeeHoldTime = RealTimeSeconds.Zero;
        private Action currentEmployeeReleaseCallback = null;

        private List<PlaceInWaitingLine> waitingLine = new();

#pragma warning disable IDE0052
        private DataProvider<WaitingLineLength> waitingLineLengthDataProvider;
#pragma warning restore IDE0052

        [SerializeField]
        private UnityEvent<EmployeeImpl> taken = new();

        [SerializeField]
        private UnityEvent<EmployeeImpl> released = new();

        public void Start()
        {
            waitingLineLengthDataProvider = new DataProvider<WaitingLineLength>(
                () => new() { Value = waitingLine.Count },
                DataProviderServiceLocator.ResolveType.MultipleSources
            );
        }

        public void Take(
            PlaceInWaitingLine place,
            RealTimeSeconds desiredTime,
            Action releasedCallback
        )
        {
            if (place == null || place.GetNextInLine() != null)
            {
                Debug.LogError("Invalid waiting line place provided");
                return;
            }

            EmployeeImpl employee = place.Employee;
            currentEmployee = employee;
            currentEmployeeHoldTime = desiredTime;
            currentEmployeeReleaseCallback = releasedCallback;

            foreach (NeedModifiers modifier in registeredModifiers)
            {
                currentEmployee.RegisterModifier(modifier);
            }

            taken?.Invoke(currentEmployee);
        }

        public void Update()
        {
            if (currentEmployee != null)
            {
                currentEmployeeHoldTime -= RealTimeSeconds.FromDeltaTime();
                if (currentEmployeeHoldTime < RealTimeSeconds.Zero)
                {
                    ReleaseEmployee();
                }
            }
        }

        public void ForceReleaseEmployeeIfAny()
        {
            if (currentEmployee == null)
            {
                return;
            }

            ReleaseEmployee();
        }

        private void ReleaseEmployee()
        {
            foreach (NeedModifiers modifier in registeredModifiers)
            {
                currentEmployee.UnregisterModifier(modifier);
            }

            if (waitingLine.Count == 0)
            {
                Debug.LogError("Cannon release employee from empty waiting line " + NeedType);
                return;
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

            EmployeeImpl previousEmployee = currentEmployee;
            currentEmployee = null;
            currentEmployeeReleaseCallback();

            released?.Invoke(previousEmployee);
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

        public void OnEmployeeFired(EmployeeImpl employee)
        {
            filter.OnEmployeeFired(employee);

            if (currentEmployee == employee)
            {
                ReleaseEmployee();
            }
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
