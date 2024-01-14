﻿using System.Collections.Generic;
using System.Linq;
using Common;
using Employee;
using Level;
using Level.Boss.Task;
using Level.Config;
using Scripts;
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
                bool all_at_meeting = employees.All(
                    employee => employee.LatestSatisfiedNeedType == NeedType.Meeting
                );
                return new AllEmployeesAtMeeting { Value = all_at_meeting };
            });
            allEmployeesAtHomeDataProvider = new DataProvider<AllEmployeesAtHome>(() =>
            {
                bool all_go_home = employees.All(
                    employee => employee.LatestSatisfiedNeedType == NeedType.Leave
                );
                return new AllEmployeesAtHome { Value = all_go_home };
            });
        }

        public Result AddEmployee(EmployeeConfig config)
        {
            var result = tileBuilderController.GrowMeetingRoomForEmployees(employees.Count + 1);

            if (result.Failure)
            {
                return result;
            }

            EmployeeImpl employee = Instantiate(employeePrototype, transform)
                .GetComponent<EmployeeImpl>();
            employee.gameObject.SetActive(true);

            // TODO: Refactor when #45 will be resolved.
            var meeting_room_places =
                FindObjectOfType<MeetingRoomLogics>() as IDataProvider<MeetingRoomPlaces>;
            var place = meeting_room_places
                .GetData()
                .Places.Where(place => place.TryTake(employee))
                .FirstOrDefault();

            if (place == null)
            {
                Destroy(employee.gameObject);
                return new FailResult("Cannot find place in meeting room");
            }

            employee.TeleportToNeedProvider(place);
            employees.Add(employee);

            return new SuccessResult();
        }
    }
}
