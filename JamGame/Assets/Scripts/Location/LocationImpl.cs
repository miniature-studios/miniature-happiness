using Common;
using Employee;
using Level;
using Level.Boss.Task;
using Level.Config;
using System.Collections.Generic;
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

        private void Start()
        {
            InitGameMode();
        }

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
            return new AllEmployeesAtMeeting { Value = true };
            // TODO:Implement
        }

        AllEmployeesAtHome IDataProvider<AllEmployeesAtHome>.GetData()
        {
            return new AllEmployeesAtHome { Value = true };
            // TODO:Implement
        }
    }
}
