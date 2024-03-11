using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Level.Config;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Level.Shop.View
{
    public partial class ViewImpl
    {
        [SerializeField]
        private EmployeeView employeeViewPrototype;

        [SerializeField]
        private Transform employeesUIContainer;

        [ReadOnly]
        [SerializeField]
        private List<EmployeeView> employeesViewList = new();

        public void OnShopEmployeesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    AddNewEmployee(e.NewItems[0] as EmployeeConfig);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    RemoveOldEmployee(e.OldItems[0] as EmployeeConfig);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    DeleteAllEmployees();
                    break;
                default:
                    Debug.LogError(
                        $"Unexpected variant of NotifyCollectionChangedAction: {e.Action}"
                    );
                    break;
            }
        }

        private void AddNewEmployee(EmployeeConfig newEmployee)
        {
            EmployeeView newEmployeeView = Instantiate(
                employeeViewPrototype,
                employeesUIContainer.transform
            );

            newEmployeeView.SetEmployeeConfig(newEmployee);
            newEmployeeView.enabled = true;
            employeesViewList.Add(newEmployeeView);
        }

        private void RemoveOldEmployee(EmployeeConfig oldEmployee)
        {
            EmployeeView employee = employeesViewList.Find(x => x.EmployeeConfig == oldEmployee);
            _ = employeesViewList.Remove(employee);
            Destroy(employee.gameObject);
        }

        private void DeleteAllEmployees()
        {
            while (employeesViewList.Count > 0)
            {
                EmployeeView item = employeesViewList.Last();
                _ = employeesViewList.Remove(item);
                Destroy(item.gameObject);
            }
        }
    }
}
