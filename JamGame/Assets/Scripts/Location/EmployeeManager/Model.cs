using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Employee;
using Employee.Needs;
using Level;
using Level.Boss.Task;
using Level.Config;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using TileBuilderController = TileBuilder.Controller;

namespace Location.EmployeeManager
{
    public struct MeetingRoomPlaces
    {
        public List<NeedProvider> Places;
    }

    [AddComponentMenu("Scripts/Location/EmployeeManager/Location.EmployeeManager.Model")]
    public class Model : MonoBehaviour
    {
        private DataProvider<EmployeeAmount> employeeAmountDataProvider;
        private DataProvider<MaxStress> maxStressDataProvider;
        private DataProvider<AllEmployeesAtMeeting> allEmployeesAtMeetingDataProvider;
        private DataProvider<AllEmployeesAtHome> allEmployeesAtHomeDataProvider;

        [SerializeField]
        private UnityEvent<EmployeeImpl> employeeFired;

        [SerializeField]
        private EmployeeImpl employeePrototype;

        [SerializeField]
        private TileBuilderController tileBuilderController;

        [ReadOnly]
        [SerializeField]
        private List<EmployeeImpl> employees = new();

        private void Start()
        {
            employeeAmountDataProvider = new DataProvider<EmployeeAmount>(
                () => new EmployeeAmount { Amount = employees.Count },
                DataProviderServiceLocator.ResolveType.Singleton
            );
            maxStressDataProvider = new DataProvider<MaxStress>(
                () =>
                {
                    float max_stress = float.NegativeInfinity;
                    foreach (EmployeeImpl emp in employees)
                    {
                        if (emp.Stress.Stress > max_stress)
                        {
                            max_stress = emp.Stress.Stress;
                        }
                    }

                    return new MaxStress { Stress = max_stress };
                },
                DataProviderServiceLocator.ResolveType.Singleton
            );
            allEmployeesAtMeetingDataProvider = new DataProvider<AllEmployeesAtMeeting>(
                () =>
                {
                    bool all_at_meeting = employees.All(employee =>
                        employee.CurrentNeedType == NeedType.Meeting
                    );
                    return new AllEmployeesAtMeeting { Value = all_at_meeting };
                },
                DataProviderServiceLocator.ResolveType.Singleton
            );
            allEmployeesAtHomeDataProvider = new DataProvider<AllEmployeesAtHome>(
                () =>
                {
                    bool all_go_home = employees.All(employee =>
                        employee.CurrentNeedType == NeedType.Leave
                    );
                    return new AllEmployeesAtHome { Value = all_go_home };
                },
                DataProviderServiceLocator.ResolveType.Singleton
            );
        }

        // TODO: Remove it when employee serialization will be implemented (#121)
        public IEnumerator TurnOnAllEmployees(float delay)
        {
            foreach (EmployeeImpl employee in employees)
            {
                employee.gameObject.SetActive(true);
                yield return delay;
            }
        }

        public Result AddEmployee(EmployeeConfig config)
        {
            Result result = tileBuilderController.GrowMeetingRoomForEmployees(employees.Count + 1);

            if (result.Failure)
            {
                return result;
            }

            EmployeeImpl employee = Instantiate(employeePrototype, transform)
                .GetComponent<EmployeeImpl>();
            employee.gameObject.SetActive(true);

            MeetingRoomPlaces meeting_room_places =
                DataProviderServiceLocator.FetchDataFromSingleton<MeetingRoomPlaces>();

            foreach (NeedProvider place in meeting_room_places.Places)
            {
                bool taken = employee.TryForceTakeNeedProvider(place);
                if (!taken)
                {
                    continue;
                }

                employees.Add(employee);
                return new SuccessResult();
            }

            Destroy(employee.gameObject);
            return new FailResult("Cannot find place in meeting room");
        }

        public void FireEmployee(EmployeeImpl employee)
        {
            if (employee.CurrentNeedType != NeedType.Meeting)
            {
                Debug.LogError("Cannot fire employee that's not on meeting");
                return;
            }

            employeeFired.Invoke(employee);
            _ = employees.Remove(employee);
            Destroy(employee.gameObject);
        }
    }
}
