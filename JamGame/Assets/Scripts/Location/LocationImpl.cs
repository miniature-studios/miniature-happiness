using Employee;
using System.Collections.Generic;
using UnityEngine;

namespace Location
{
    public class LocationImpl : MonoBehaviour
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
    }
}
