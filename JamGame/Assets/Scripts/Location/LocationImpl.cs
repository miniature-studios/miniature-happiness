using Common;
using Employee;
using Level.Boss.Task;
using System.Collections.Generic;
using UnityEngine;

namespace Location
{
    [AddComponentMenu("Location.Location")]
    public class LocationImpl : MonoBehaviour, IDataProvider<EmployeeAmount>, IDataProvider<MaxStress>
    {
        [SerializeField]
        private EmployeeImpl employeePrototype;
        private List<NeedProvider> needProviders;
        private readonly List<EmployeeImpl> employees = new();

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

        public void AddEmployee()
        {
            EmployeeImpl new_employee = Instantiate(
                employeePrototype,
                employeePrototype.transform.parent
            );
            new_employee.gameObject.SetActive(true);
            employees.Add(new_employee);
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
    }
}
