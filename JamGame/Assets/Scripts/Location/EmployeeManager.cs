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
using TileBuilderController = TileBuilder.Controller.ControllerImpl;

namespace Location
{
    public struct MeetingRoomPlaces
    {
        public List<NeedProvider> Places;
    }

    [AddComponentMenu("Scripts/Location/Location.EmployeeManager")]
    public class EmployeeManager : MonoBehaviour
    {
        private DataProvider<EmployeeAmount> employeeAmountDataProvider;
        private DataProvider<MaxStress> maxStressDataProvider;
        private DataProvider<AllEmployeesAtMeeting> allEmployeesAtMeetingDataProvider;
        private DataProvider<AllEmployeesAtHome> allEmployeesAtHomeDataProvider;

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
                () => new EmployeeAmount { Amount = employees.Count }
            );
            maxStressDataProvider = new DataProvider<MaxStress>(() =>
            {
                float max_stress = float.NegativeInfinity;
                foreach (EmployeeImpl emp in employees)
                {
                    if (emp.Stress.Value > max_stress)
                    {
                        max_stress = emp.Stress.Value;
                    }
                }

                return new MaxStress { Stress = max_stress };
            });
            allEmployeesAtMeetingDataProvider = new DataProvider<AllEmployeesAtMeeting>(() =>
            {
                bool all_at_meeting = employees.All(employee =>
                    employee.CurrentNeedType == NeedType.Meeting
                );
                return new AllEmployeesAtMeeting { Value = all_at_meeting };
            });
            allEmployeesAtHomeDataProvider = new DataProvider<AllEmployeesAtHome>(() =>
            {
                bool all_go_home = employees.All(employee =>
                    employee.CurrentNeedType == NeedType.Leave
                );
                return new AllEmployeesAtHome { Value = all_go_home };
            });
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

                // TODO: set parent back when meeting ends.
                employee.transform.SetParent(place.transform, true);
                employees.Add(employee);
                return new SuccessResult();
            }

            Destroy(employee.gameObject);
            return new FailResult("Cannot find place in meeting room");
        }
    }
}
