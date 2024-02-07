using System.Collections.Generic;
using System.Linq;
using Common;
using Employee;
using Employee.Needs;
using Level;
using Level.Boss.Task;
using Level.Config;
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
        private DataProvider<MaxStress> maxStreessDataProvider;
        private DataProvider<AllEmployeesAtMeeting> allEmployeesAtMeetingDataProvider;
        private DataProvider<AllEmployeesAtHome> allEmployeesAtHomeDataProvider;

        [SerializeField]
        private EmployeeImpl employeePrototype;

        [SerializeField]
        private TileBuilderController tileBuilderController;

        private List<EmployeeImpl> employees = new();

        private void Start()
        {
            employeeAmountDataProvider = new DataProvider<EmployeeAmount>(
                () => new EmployeeAmount { Amount = employees.Count }
            );
            maxStreessDataProvider = new DataProvider<MaxStress>(() =>
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
                    employee.LatestSatisfiedNeedType == NeedType.Meeting
                );
                return new AllEmployeesAtMeeting { Value = all_at_meeting };
            });
            allEmployeesAtHomeDataProvider = new DataProvider<AllEmployeesAtHome>(() =>
            {
                bool all_go_home = employees.All(employee =>
                    employee.LatestSatisfiedNeedType == NeedType.Leave
                );
                return new AllEmployeesAtHome { Value = all_go_home };
            });
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
                NeedProvider.Reservation reservation = place.TryReserve(employee);
                if (reservation == null)
                {
                    continue;
                }

                employee.TeleportToNeedProvider(place);
                place.Take(reservation);
                employees.Add(employee);

                return new SuccessResult();
            }

            Destroy(employee.gameObject);
            return new FailResult("Cannot find place in meeting room");
        }
    }
}
