using Common;
using Level.Boss.Task;
using Level;
using System.Collections.Generic;
using UnityEngine;
using Employee;
using Level.Config;
using System.Linq;
using TileBuilderController = TileBuilder.Controller.ControllerImpl;
using Pickle;
using Sirenix.OdinInspector;
using TileUnion;

namespace Location
{
    public struct MeetingRoomPlaces
    {
        public List<NeedProvider> Places;
    }

    [AddComponentMenu("Scripts/Location.EmployeeManager")]
    public class EmployeeManager : MonoBehaviour,
            IDataProvider<EmployeeAmount>,
            IDataProvider<MaxStress>,
            IDataProvider<AllEmployeesAtMeeting>,
            IDataProvider<AllEmployeesAtHome>
    {
        [SerializeField]
        private EmployeeImpl employeePrototype;

        [SerializeField]
        private TileBuilderController tileBuilderController;

        private List<EmployeeImpl> employees = new();

        public Result AddEmployee(EmployeeConfig config)
        {
            var result = tileBuilderController.GrowMeetingRoomForEmployees(employees.Count + 1);
                
            if(result.Failure)
            {
                return result;
            }

            EmployeeImpl employee = Instantiate(employeePrototype, transform)
                .GetComponent<EmployeeImpl>();
            employee.gameObject.SetActive(true);

            // TODO: Refactor when #45 will be resolved.
            var meeting_room_places = FindObjectOfType<MeetingRoomLogics>() as IDataProvider<MeetingRoomPlaces>;
            var place = meeting_room_places
                .GetData()
                .Places
                .Where(place => place.TryTake(employee))
                .FirstOrDefault();

            if(place == null)
            {
                Destroy(employee.gameObject);
                return new FailResult("Cannot find place in meeting room");
            }

            employee.TeleportToNeedProvider(place);
            employees.Add(employee);

            return new SuccessResult();
        }

        EmployeeAmount IDataProvider<EmployeeAmount>.GetData()
        {
            return new EmployeeAmount { Amount = employees.Count };
        }

        MaxStress IDataProvider<MaxStress>.GetData()
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
        }

        AllEmployeesAtMeeting IDataProvider<AllEmployeesAtMeeting>.GetData()
        {
            bool all_at_meeting = employees.All(
                employee => employee.LatestSatisfiedNeedType == NeedType.Meeting
            );
            return new AllEmployeesAtMeeting { Value = all_at_meeting };
        }

        AllEmployeesAtHome IDataProvider<AllEmployeesAtHome>.GetData()
        {
            bool all_go_home = employees.All(
                employee => employee.LatestSatisfiedNeedType == NeedType.Leave
            );
            return new AllEmployeesAtHome { Value = all_go_home };
        }
    }
}
