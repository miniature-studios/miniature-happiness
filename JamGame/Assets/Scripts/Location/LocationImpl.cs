using System.Collections.Generic;
using System.Linq;
using Common;
using Employee;
using Level;
using Level.Boss.Task;
using Level.Config;
using UnityEngine;

namespace Location
{
    [AddComponentMenu("Scripts/Location.Location")]
    public class LocationImpl
        : MonoBehaviour,
            IDataProvider<EmployeeAmount>,
            IDataProvider<MaxStress>,
            IDataProvider<AllEmployeesAtMeeting>,
            IDataProvider<AllEmployeesAtHome>
    {
        [SerializeField]
        private EmployeeImpl employeePrototype;
        private List<NeedProvider> needProviders;
        private List<EmployeeImpl> employees = new();

        // TODO: Call it each time room added/removed.
        public void InitGameMode()
        {
            needProviders = new List<NeedProvider>(
                transform.GetComponentsInChildren<NeedProvider>()
            );
        }

        public void AddEmployee(EmployeeConfig config)
        {
            EmployeeImpl employee = Instantiate(config.Prototype, transform)
                .GetComponent<EmployeeImpl>();
            employee.gameObject.SetActive(true);
            employees.Add(employee);
        }

        public IEnumerable<NeedProvider> FindAllAvailableProviders(
            EmployeeImpl employee,
            NeedType need_type
        )
        {
            foreach (NeedProvider provider in needProviders)
            {
                if (provider.NeedType == need_type && provider.IsAvailable(employee))
                {
                    yield return provider;
                }
            }
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
